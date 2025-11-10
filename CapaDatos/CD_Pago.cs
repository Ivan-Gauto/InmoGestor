using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_Pago
    {
        // 🔹 Listar todos los pagos (puede usarse para la vista del gerente o panel general)
        public List<Pago> ListarTodos()
        {
            var list = new List<Pago>();

            const string sql = @"
SELECT 
    pago_id, fecha_pago, monto_total, periodo,
    metodo_pago_id, estado, cuota_id, mora_cobrada, usuario_creador
FROM dbo.pago
ORDER BY fecha_pago DESC;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var p = new Pago();
                        p.PagoId = Convert.ToInt64(dr["pago_id"]);
                        p.FechaPago = Convert.ToDateTime(dr["fecha_pago"]);
                        p.MontoTotal = Convert.ToDecimal(dr["monto_total"]);
                        p.Periodo = dr["periodo"].ToString();
                        p.MetodoPagoId = Convert.ToInt32(dr["metodo_pago_id"]);
                        p.Estado = Convert.ToInt32(dr["estado"]);
                        p.CuotaId = Convert.ToInt32(dr["cuota_id"]);
                        p.MoraCobrada = Convert.ToDecimal(dr["mora_cobrada"]);
                        p.UsuarioCreador = Convert.ToInt32(dr["usuario_creador"]);

                        list.Add(p);
                    }
                }
            }

            return list;
        }

        // 🔹 Listar pagos por cuota (para mostrar pagos vinculados a una cuota específica)
        public List<Pago> ListarPorCuota(int cuotaId)
        {
            var list = new List<Pago>();

            const string sql = @"
SELECT 
    pago_id, fecha_pago, monto_total, periodo,
    metodo_pago_id, estado, cuota_id, mora_cobrada, usuario_creador
FROM dbo.pago
WHERE cuota_id = @id
ORDER BY fecha_pago DESC;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@id", cuotaId);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var p = new Pago();
                        p.PagoId = Convert.ToInt64(dr["pago_id"]);
                        p.FechaPago = Convert.ToDateTime(dr["fecha_pago"]);
                        p.MontoTotal = Convert.ToDecimal(dr["monto_total"]);
                        p.Periodo = dr["periodo"].ToString();
                        p.MetodoPagoId = Convert.ToInt32(dr["metodo_pago_id"]);
                        p.Estado = Convert.ToInt32(dr["estado"]);
                        p.CuotaId = Convert.ToInt32(dr["cuota_id"]);
                        p.MoraCobrada = Convert.ToDecimal(dr["mora_cobrada"]);
                        p.UsuarioCreador = Convert.ToInt32(dr["usuario_creador"]);

                        list.Add(p);
                    }
                }
            }

            return list;
        }

        // 🔹 Registrar un nuevo pago (usa defaults de la BD)
        public int Registrar(Pago obj)
        {
            int idGenerado = 0;

            const string sql = @"
INSERT INTO dbo.pago (fecha_pago, monto_total, periodo, metodo_pago_id, estado, cuota_id, mora_cobrada, usuario_creador)
OUTPUT inserted.pago_id
VALUES (@fecha, @monto, @periodo, @metodo, @estado, @cuota, @mora, @usuario);";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@fecha", obj.FechaPago);
                cmd.Parameters.AddWithValue("@monto", obj.MontoTotal);
                cmd.Parameters.AddWithValue("@periodo", obj.Periodo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@metodo", obj.MetodoPagoId);
                cmd.Parameters.AddWithValue("@estado", obj.Estado);
                cmd.Parameters.AddWithValue("@cuota", obj.CuotaId);
                cmd.Parameters.AddWithValue("@mora", obj.MoraCobrada);
                cmd.Parameters.AddWithValue("@usuario", obj.UsuarioCreador);

                cn.Open();
                idGenerado = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return idGenerado;
        }

        // 🔹 Cambiar estado del pago (0,1,2)
        public bool CambiarEstado(long pagoId, int nuevoEstado)
        {
            bool resultado = false;

            const string sql = @"
UPDATE dbo.pago 
SET estado = @estado
WHERE pago_id = @id;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@id", pagoId);

                cn.Open();
                resultado = cmd.ExecuteNonQuery() > 0;
            }

            return resultado;
        }

        // 🔹 Obtener un pago puntual
        public Pago ObtenerPorId(long id)
        {
            Pago p = null;

            const string sql = @"
SELECT 
    pago_id, fecha_pago, monto_total, periodo,
    metodo_pago_id, estado, cuota_id, mora_cobrada, usuario_creador
FROM dbo.pago
WHERE pago_id = @id;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        p = new Pago();
                        p.PagoId = Convert.ToInt64(dr["pago_id"]);
                        p.FechaPago = Convert.ToDateTime(dr["fecha_pago"]);
                        p.MontoTotal = Convert.ToDecimal(dr["monto_total"]);
                        p.Periodo = dr["periodo"].ToString();
                        p.MetodoPagoId = Convert.ToInt32(dr["metodo_pago_id"]);
                        p.Estado = Convert.ToInt32(dr["estado"]);
                        p.CuotaId = Convert.ToInt32(dr["cuota_id"]);
                        p.MoraCobrada = Convert.ToDecimal(dr["mora_cobrada"]);
                        p.UsuarioCreador = Convert.ToInt32(dr["usuario_creador"]);
                    }
                }
            }

            return p;
        }
    }
}
