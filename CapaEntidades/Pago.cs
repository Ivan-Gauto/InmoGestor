using System;

namespace CapaEntidades
{
    /// <summary>
    /// Representa un registro de la tabla dbo.PAGO
    /// Estados: 0 = Anulado, 1 = Confirmado, 2 = En solicitud de aprobación
    /// </summary>
    public class Pago
    {
        public long PagoId { get; set; }                 // pago_id (PK, identity)
        public DateTime FechaPago { get; set; }          // fecha_pago (default GETDATE() en BD)
        public decimal MontoTotal { get; set; }          // monto_total (decimal(12,2))
        public string Periodo { get; set; }              // periodo (varchar(50))
        public int MetodoPagoId { get; set; }            // metodo_pago_id (FK)
        public int Estado { get; set; }                  // estado (0=Anulado,1=Confirmado,2=Solicitud) default 2
        public int CuotaId { get; set; }                 // cuota_id (FK)
        public decimal MoraCobrada { get; set; }         // mora_cobrada (decimal(10,2)) default 0
        public int UsuarioCreador { get; set; }          // usuario_creador (FK)
    }

    /// <summary>
    /// Ayuda para legibilidad al trabajar con Estado (opcional)
    /// </summary>
    public enum PagoEstado
    {
        Anulado = 0,
        Confirmado = 1,
        SolicitudAprobacion = 2
    }
}
