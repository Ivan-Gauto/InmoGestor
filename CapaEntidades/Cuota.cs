using System;

namespace CapaEntidades
{
    public class Cuota
    {
        // --- PK
        public int CuotaId { get; set; }

        // --- Datos base
        public int ContratoId { get; set; }
        public int NroCuota { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaVenc { get; set; }
        public decimal ImporteBase { get; set; }
        public byte Estado { get; set; } = 0; // 0 = Pendiente
        public decimal TasaMoraMensualAplicada { get; set; }
        public decimal OtrosAdicionalesTotal { get; set; }
        public string DescAdicionales { get; set; } = string.Empty;

        // --- Referencia a contrato
        public ContratoAlquiler oContrato { get; set; } = new ContratoAlquiler();

        // --- Propiedades derivadas (no mapeadas)
        public string EstadoTexto
        {
            get
            {
                switch (Estado)
                {
                    case 0:
                        return "Pendiente";
                    case 1:
                        return "Vencida";
                    case 2:
                        return "Pagada";
                    case 3:
                        return "Anulada";
                    default:
                        return "Desconocido";
                }
            }
        }


        public string PeriodoFormateado
        {
            get
            {
                if (Periodo.Length == 6)
                {
                    string anio = Periodo.Substring(0, 4);
                    string mes = Periodo.Substring(4, 2);
                    return $"{mes}/{anio}";
                }
                return Periodo;
            }
        }
    }
}
