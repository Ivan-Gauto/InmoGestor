// CapaDatos/CD_Recibo.cs
using CapaEntidades;
using System;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Recibo
    {
        public long CrearParaPago(long pagoId)
        {
            if (pagoId <= 0) throw new ArgumentException("pagoId inválido.");

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(@"
                INSERT INTO dbo.recibo (pago_id, fecha_emision)
                OUTPUT INSERTED.recibo_id
                VALUES (@pago_id, GETDATE());
            ", cn))
            {
                cmd.Parameters.AddWithValue("@pago_id", pagoId);
                cn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt64(result);
            }
        }

        public Recibo ObtenerPorPago(long pagoId)
        {
            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 recibo_id, pago_id, fecha_emision, nro_comprobante
                FROM dbo.recibo
                WHERE pago_id = @pago_id
                ORDER BY recibo_id DESC;", cn))
            {
                cmd.Parameters.AddWithValue("@pago_id", pagoId);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;
                    return new Recibo
                    {
                        ReciboId = rd.GetInt64(0),
                        PagoId = rd.GetInt64(1),
                        FechaEmision = rd.GetDateTime(2),
                        NroComprobante = rd.GetInt32(3)
                    };
                }
            }
        }

        public bool ExisteParaPago(long pagoId)
        {
            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(@"
                SELECT 1 FROM dbo.recibo WITH (NOLOCK) WHERE pago_id = @pago_id;
            ", cn))
            {
                cmd.Parameters.AddWithValue("@pago_id", pagoId);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                    return rd.Read();
            }
        }
    }
}
