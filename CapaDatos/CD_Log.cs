using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;
using System.Text;

namespace CapaDatos
{
    public class CD_Log
    {
        public void RegistrarAuditoriaUsuario(string adminEjecutorDni, string usuarioAfectadoDni, string accion, string detalle)
        {
            try
            {
                using (var cn = new SqlConnection(Conexion.cadena))
                {
                    string query = @"
                        INSERT INTO dbo.log_usuario (admin_ejecutor_dni, usuario_afectado_dni, accion, detalle)
                        VALUES (@adminDni, @afectadoDni, @accion, @detalle)";

                    using (var cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@adminDni", (object)adminEjecutorDni ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@afectadoDni", (object)usuarioAfectadoDni ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@accion", accion);
                        cmd.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                        cmd.CommandType = CommandType.Text;

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Considera registrar este error en log_error
            }
        }

        public List<LogUsuario> ListarAuditoriaUsuarios(string adminDni, DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<LogUsuario>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                var queryBuilder = new StringBuilder(@"
                    SELECT log_usuario_id, fecha_hora, admin_ejecutor_dni,
                           usuario_afectado_dni, accion, detalle
                    FROM dbo.log_usuario
                    WHERE fecha_hora BETWEEN @fechaInicio AND @fechaFin");

                var cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);

                if (!string.IsNullOrEmpty(adminDni) && adminDni != "0") // Verifica que no sea null ni el valor "Todos"
                {
                    queryBuilder.Append(" AND admin_ejecutor_dni = @adminDni");
                    cmd.Parameters.AddWithValue("@adminDni", adminDni);
                }

                queryBuilder.Append(" ORDER BY fecha_hora DESC");

                cmd.CommandText = queryBuilder.ToString();
                cmd.Connection = cn;

                try
                {
                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new LogUsuario
                            {
                                LogUsuarioId = Convert.ToInt64(dr["log_usuario_id"]),
                                FechaHora = Convert.ToDateTime(dr["fecha_hora"]),
                                AdminEjecutorDni = dr["admin_ejecutor_dni"]?.ToString(),
                                UsuarioAfectadoDni = dr["usuario_afectado_dni"]?.ToString(),
                                Accion = dr["accion"].ToString(),
                                Detalle = dr["detalle"]?.ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Considera registrar este error en log_error
                    lista = new List<LogUsuario>();
                }
            }
            return lista;
        }

        public void RegistrarError(string usuarioDni, string formulario, string metodo, string mensajeError, string stackTrace = null)
        {
            try
            {
                using (var cn = new SqlConnection(Conexion.cadena))
                {
                    string query = @"
                         INSERT INTO dbo.log_error (usuario_id, formulario, metodo, mensaje_error, stack_trace)
                         SELECT u.usuario_id, @form, @metodo, @msg, @stack
                         FROM dbo.usuario u WHERE u.dni = @dni";

                    using (var cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@dni", (object)usuarioDni ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@form", (object)formulario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@metodo", (object)metodo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@msg", mensajeError);
                        cmd.Parameters.AddWithValue("@stack", (object)stackTrace ?? DBNull.Value);
                        cmd.CommandType = CommandType.Text;

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Si el log de errores falla...
            }
        }
    }
}