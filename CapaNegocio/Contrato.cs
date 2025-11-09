using System;
using System.Collections.Generic;
using CapaEntidades;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Contrato
    {
        /// <summary>
        /// Valida los datos clave del contrato antes de persistir.
        /// </summary>
        ///     
        CD_Contrato cdContrato = new CD_Contrato();
        private string ValidarContrato(ContratoAlquiler c)
        {
            if (c.InmuebleId <= 0) return "Debe seleccionar un inmueble.";
            if (string.IsNullOrWhiteSpace(c.DniInquilino)) return "Debe seleccionar un inquilino.";
            if (c.CantidadCuotas <= 0) return "La cantidad de cuotas debe ser mayor a cero.";
            if (c.PrecioCuota <= 0) return "El precio de la cuota debe ser mayor a cero.";
            if (c.TasaMoraMensual < 0) return "La tasa de mora no puede ser negativa.";
            if (c.FechaInicio.Date > c.FechaFin.Date) return "La fecha de inicio no puede ser mayor a la de fin.";
            return string.Empty;
        }

        /// <summary>
        /// Genera N cuotas a partir de la fecha de inicio.
        /// Regla de vencimiento: mismo día de inicio; si el mes no lo tiene, usa último día del mes.
        /// </summary>
        public List<Cuota> GenerarCronograma(DateTime fechaInicio, int cantidadCuotas,
                                            decimal precioCuota, decimal tasaMoraMensual,
                                            string descAdicionales = "")
        {
            List<Cuota> cuotas = new List<Cuota>();
            int diaVenc = fechaInicio.Day;

            for (int i = 0; i < cantidadCuotas; i++)
            {
                DateTime periodoDate = fechaInicio.AddMonths(i);
                DateTime fechaVenc = CalcularVencimiento(periodoDate.Year, periodoDate.Month, diaVenc);
                string periodo = FormatearPeriodo(periodoDate); // "AAAAMM"

                Cuota q = new Cuota
                {
                    NroCuota = i + 1,
                    Periodo = periodo,
                    FechaVenc = fechaVenc,
                    ImporteBase = precioCuota,
                    Estado = 0, // 0 = Pendiente
                    TasaMoraMensualAplicada = tasaMoraMensual,
                    OtrosAdicionalesTotal = 0m,
                    DescAdicionales = descAdicionales ?? string.Empty
                };

                cuotas.Add(q);
            }

            return cuotas;
        }

        /// <summary>
        /// Orquesta: valida, genera cuotas y delega en Capa Datos (transacción adentro).
        /// </summary>
        public bool RegistrarContrato(ContratoAlquiler contrato, out int contratoId, out string mensaje)
        {
            contratoId = 0;
            mensaje = string.Empty;

            CD_Contrato cdContrato = new CD_Contrato();
            CD_Cuota cdCuota = new CD_Cuota();

            // Sugerencia: si la UI no setea FechaFin, calcularla automáticamente
            if (contrato.FechaFin == default(DateTime))
            {
                // Fin = último día del último mes del contrato
                DateTime fin = contrato.FechaInicio.AddMonths(contrato.CantidadCuotas);
                contrato.FechaFin = new DateTime(fin.Year, fin.Month, DateTime.DaysInMonth(fin.Year, fin.Month));
            }

            // Estado por defecto (si la BD no tiene DEFAULT forzado)
            if (contrato.Estado == 0)
                contrato.Estado = 1; // 1 = Vigente

            // Validaciones de negocio
            string error = ValidarContrato(contrato);
            if (!string.IsNullOrEmpty(error))
            {
                mensaje = error;
                return false;
            }

            // Generar cronograma
            List<Cuota> cuotas = GenerarCronograma(
                contrato.FechaInicio,
                contrato.CantidadCuotas,
                contrato.PrecioCuota,
                contrato.TasaMoraMensual,
                contrato.Condiciones // opcional como descripción genérica
            );

            // Persistir (transacción en CD_Contrato)
            return cdContrato.RegistrarContratoYCuotas(contrato, cuotas, out contratoId, out mensaje);
        }

        /// <summary>
        /// Borrado lógico del contrato (estado = 0) + inmueble vuelve a disponible.
        /// Cuotas no se tocan (registro histórico).
        /// </summary>
        public bool AnularContrato(int contratoId, out string mensaje)
        {
            CD_Contrato cdContrato = new CD_Contrato();
            return cdContrato.AnularContrato(contratoId, out mensaje);
        }

        /// <summary>
        /// Helpers
        /// </summary>
        private static string FormatearPeriodo(DateTime fecha) // "AAAAMM"
        {
            return fecha.Year.ToString("0000") + fecha.Month.ToString("00");
        }

        private static DateTime CalcularVencimiento(int anio, int mes, int diaDeseado)
        {
            int ultimoDia = DateTime.DaysInMonth(anio, mes);
            int dia = diaDeseado > ultimoDia ? ultimoDia : diaDeseado;
            return new DateTime(anio, mes, dia);
        }

    }
}
