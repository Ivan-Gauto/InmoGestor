using System;

namespace CapaEntidades
{
    public class Recibo
    {
        public long ReciboId { get; set; }
        public long PagoId { get; set; }
        public DateTime FechaEmision { get; set; }

        public string Periodo { get; set; }           // Copiado desde cuota
        public decimal ImporteBase { get; set; }      // Copiado desde cuota
        public decimal AdicionalMora { get; set; }    // pago.mora_cobrada
        public decimal OtrosAdicionales { get; set; } // total - base - mora
        public decimal Total { get; set; }            // pago.monto_total
        public int MetodoPagoId { get; set; }         // pago.metodo_pago_id}

        public int NroComprobante { get; set; }         
    }
}
