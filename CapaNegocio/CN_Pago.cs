using System;
using System.Collections.Generic;
using CapaEntidades;
using CapaDatos;

namespace CapaNegocio
{
    public static class Roles
    {
        public const int Gerente = 2;
        public const int Operador = 3;
        // Agregá otros si aparecen (Admin=1, etc.)
    }

    /// <summary>
    /// Reglas de negocio para PAGO.
    /// - Solo Gerente (rol 2) puede anular.
    /// - No hay edición de pagos (la CN no expone métodos para editar campos).
    /// - Controla transiciones de estado:
    ///     2 -> 1  Confirmar
    ///     1 -> 2  Volver a solicitud
    ///     1/2 -> 0  Anular
    /// </summary>
    public class CN_Pago
    {
        private readonly CD_Pago _cdPago = new CD_Pago();
        private readonly CD_Cuota _cdCuota = new CD_Cuota();


        #region Lecturas

        public List<Pago> ListarTodos()
        {
            return _cdPago.ListarTodos();
        }

        public List<Pago> ListarPorCuota(int cuotaId)
        {
            if (cuotaId <= 0) throw new ArgumentException("cuotaId inválido.");
            return _cdPago.ListarPorCuota(cuotaId);
        }

        public Pago ObtenerPorId(long pagoId)
        {
            if (pagoId <= 0) throw new ArgumentException("pagoId inválido.");
            return _cdPago.ObtenerPorId(pagoId);
        }

        #endregion

        #region Crear (no edita, solo inserta)

        /// <summary>
        /// Registra un nuevo pago.
        /// - Si no te interesa fijar estado desde UI, pasá Estado=2 (solicitud).
        /// - No valida lógica de cupos por cuota (lo hacemos si lo pedís).
        /// </summary>
        public long RegistrarPago(Pago nuevo, int rolUsuario)
        {
            if (nuevo == null) throw new ArgumentNullException(nameof(nuevo));
            ValidarEstructuraPago(nuevo);

            if (nuevo.FechaPago == default(DateTime))
                nuevo.FechaPago = DateTime.Today;

            var id = _cdPago.Registrar(nuevo);
            if (id <= 0) throw new InvalidOperationException("No se pudo registrar el pago.");
            // Si el pago entra como Confirmado (1), marcamos la cuota como Pagada (2)
            if (nuevo.Estado == 1)
            {
                _cdCuota.CambiarEstado(nuevo.CuotaId, 2);
            }

            return id;
        }

        #endregion

        #region Cambios de estado (sin edición de campos)

        /// <summary>
        /// Confirma un pago: 2 -> 1. (Idempotente si ya está en 1)
        /// Operador (3) y Gerente (2) pueden confirmar.
        /// </summary>
        public bool ConfirmarPago(long pagoId, int rolUsuario)
        {
            if (pagoId <= 0) throw new ArgumentException("pagoId inválido.");

            if (rolUsuario != Roles.Gerente)
                throw new UnauthorizedAccessException("Solo un GERENTE (rol 2) puede confirmar pagos.");

            var actual = _cdPago.ObtenerPorId(pagoId);
            if (actual == null) throw new InvalidOperationException("Pago no encontrado.");

            if (actual.Estado == 0)
                throw new InvalidOperationException("No se puede confirmar un pago ANULADO.");

            if (actual.Estado == 1)
                return true; // idempotente

            if (actual.Estado != 2)
                throw new InvalidOperationException("Solo se puede confirmar un pago en 'Solicitud de aprobación'.");

            var ok = _cdPago.CambiarEstado(pagoId, 1);
            if (ok)
            {
                // ahora sí: la cuota queda Pagada (2)
                _cdCuota.CambiarEstado(actual.CuotaId, 2);
            }
            return ok;

        }

        /// <summary>
        /// Vuelve a 'Solicitud de aprobación': 1 -> 2 (idempotente si ya está en 2).
        /// Operador (3) y Gerente (2) pueden.
        /// </summary>
        public bool SolicitarAprobacion(long pagoId, int rolUsuario)
        {
            if (pagoId <= 0) throw new ArgumentException("pagoId inválido.");

            var actual = _cdPago.ObtenerPorId(pagoId);
            if (actual == null) throw new InvalidOperationException("Pago no encontrado.");

            if (actual.Estado == 0)
                throw new InvalidOperationException("No se puede volver a solicitud un pago ANULADO.");

            if (actual.Estado == 2)
                return true; // idempotente

            if (actual.Estado != 1)
                throw new InvalidOperationException("Solo se puede pasar a solicitud desde 'Confirmado'.");

            return _cdPago.CambiarEstado(pagoId, 2);
        }

        /// <summary>
        /// Anula un pago: 1/2 -> 0 (idempotente si ya está en 0).
        /// Solo Gerente (rol 2) puede anular.
        /// </summary>
        public bool AnularPago(long pagoId, int rolUsuario)
        {
            if (pagoId <= 0) throw new ArgumentException("pagoId inválido.");
            if (rolUsuario != Roles.Gerente)
                throw new UnauthorizedAccessException("Solo un GERENTE (rol 2) puede anular pagos.");

            var actual = _cdPago.ObtenerPorId(pagoId);
            if (actual == null) throw new InvalidOperationException("Pago no encontrado.");

            if (actual.Estado == 0)
                return true; // idempotente

            if (actual.Estado != 1 && actual.Estado != 2)
                throw new InvalidOperationException("Solo se puede anular un pago en estado 'Confirmado' o 'Solicitud de aprobación'.");

            // Si luego querés registrar en dbo.pago_anulado, agregamos inserción aquí.
            return _cdPago.CambiarEstado(pagoId, 0);
        }

        #endregion

        #region Validaciones

        private static void ValidarEstructuraPago(Pago p)
        {
            if (p.CuotaId <= 0) throw new ArgumentException("CuotaId inválido.");
            if (p.MetodoPagoId <= 0) throw new ArgumentException("MetodoPagoId inválido.");
            if (p.UsuarioCreador <= 0) throw new ArgumentException("UsuarioCreador inválido.");
            if (p.MontoTotal < 0) throw new ArgumentException("MontoTotal no puede ser negativo.");
            if (p.MoraCobrada < 0) throw new ArgumentException("MoraCobrada no puede ser negativa.");
            if (p.Estado < 0 || p.Estado > 2) throw new ArgumentException("Estado inválido. Debe ser 0, 1 o 2.");
            if (!string.IsNullOrEmpty(p.Periodo) && p.Periodo.Length > 50)
                throw new ArgumentException("Periodo excede el máximo de 50 caracteres.");
        }

        #endregion
    }
}
