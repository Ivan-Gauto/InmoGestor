using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_Cuota
    {
        public List<Cuota> ListarPorContrato(int contratoId)
        {
            var list = new List<Cuota>();

            const string sql = @"
SELECT 
    cuota_id, contrato_id, nro_cuota, periodo, fecha_venc,
    importe_base, estado, tasa_mora_mensual_aplicada,
    otros_adicionales_total, desc_adicionales
FROM dbo.cuota
WHERE contrato_id = @id
ORDER BY nro_cuota;";

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@id", contratoId);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var q = new Cuota();
                        q.CuotaId = Convert.ToInt32(dr["cuota_id"]);
                        q.ContratoId = Convert.ToInt32(dr["contrato_id"]);
                        q.NroCuota = Convert.ToInt32(dr["nro_cuota"]);
                        q.Periodo = dr["periodo"].ToString();
                        q.FechaVenc = Convert.ToDateTime(dr["fecha_venc"]);
                        q.ImporteBase = Convert.ToDecimal(dr["importe_base"]);
                        q.Estado = Convert.ToByte(dr["estado"]);
                        q.TasaMoraMensualAplicada = Convert.ToDecimal(dr["tasa_mora_mensual_aplicada"]);
                        q.OtrosAdicionalesTotal = Convert.ToDecimal(dr["otros_adicionales_total"]);
                        q.DescAdicionales = dr["desc_adicionales"] == DBNull.Value ? string.Empty : dr["desc_adicionales"].ToString();

                        list.Add(q);
                    }
                }
            }

            return list;
        }

        public bool CambiarEstado(int cuotaId, byte nuevoEstado)
        {
            try
            {
                using (var cn = new SqlConnection(Conexion.cadena))
                using (var cmd = new SqlCommand("UPDATE dbo.cuota SET estado = @e WHERE cuota_id = @id;", cn))
                {
                    cmd.Parameters.AddWithValue("@e", nuevoEstado);
                    cmd.Parameters.AddWithValue("@id", cuotaId);
                    cn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }


    }
}
