using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_PersonaRolCliente
    {
        public List<PersonaRolCliente> Listar(int rolClienteId, bool? activos = null)
        {
            var lista = new List<PersonaRolCliente>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                var queryBuilder = new StringBuilder(@"
            SELECT prc.fecha_creacion, prc.dni, prc.rol_cliente_id,
                   p.nombre, p.apellido, p.correo_electronico, p.telefono,
                   p.direccion, p.estado, p.fecha_nacimiento
            FROM persona_rol_cliente prc
            INNER JOIN persona p ON p.dni = prc.dni
            WHERE prc.rol_cliente_id = @rol_cliente_id");

                if (activos.HasValue)
                {
                    queryBuilder.Append(" AND p.estado = @estado");
                }

                using (var cmd = new SqlCommand(queryBuilder.ToString(), cn))
                {
                    cmd.Parameters.AddWithValue("@rol_cliente_id", rolClienteId);
                    if (activos.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@estado", activos.Value ? 1 : 0);
                    }

                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PersonaRolCliente
                            {
                                FechaCreacion = dr.GetDateTime(dr.GetOrdinal("fecha_creacion")),
                                Dni = dr["dni"]?.ToString() ?? "",
                                oRolCliente = new RolCliente
                                {
                                    RolClienteId = Convert.ToInt32(dr["rol_cliente_id"]),
                                },
                                oPersona = new Persona
                                {
                                    Dni = dr["dni"]?.ToString() ?? "",
                                    Nombre = dr["nombre"]?.ToString() ?? "",
                                    Apellido = dr["apellido"]?.ToString() ?? "",
                                    CorreoElectronico = dr["correo_electronico"]?.ToString() ?? "",
                                    Telefono = dr["telefono"]?.ToString() ?? "",
                                    Direccion = dr["direccion"]?.ToString() ?? "",
                                    Estado = Convert.ToInt32(dr["estado"]),
                                    FechaNacimiento = dr.GetDateTime(dr.GetOrdinal("fecha_nacimiento"))
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool CambiarEstado(string dni, int nuevoEstado)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                {
                    // 1. La consulta SQL es un UPDATE simple sobre la tabla 'persona'.
                    string query = "UPDATE persona SET Estado = @estado WHERE Dni = @dni";

                    SqlCommand cmd = new SqlCommand(query, cn);

                    // 2. Se asignan los valores a los parámetros para seguridad.
                    cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.CommandType = CommandType.Text;

                    cn.Open();

                    // 3. ExecuteNonQuery devuelve el número de filas afectadas.
                    //    Si es mayor a 0, significa que la actualización se realizó con éxito.
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                // En caso de un error en la base de datos, la operación falla.
                // Aquí podrías registrar el error 'ex' en un archivo de log si quisieras.
                return false;
            }
        }

        public bool Registrar(PersonaRolCliente prc, out string mensaje)
        {
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        var cmdPersona = new SqlCommand(@"
                            IF EXISTS (SELECT 1 FROM persona WHERE dni=@dni)
                            BEGIN
                                UPDATE persona
                                   SET nombre             = @nombre,
                                       apellido           = @apellido,
                                       correo_electronico = @correo,
                                       telefono           = @tel,
                                       direccion          = @dir,
                                       estado             = @estadoPersona,
                                       fecha_nacimiento   = @fnac
                                 WHERE dni = @dni;
                            END
                            ELSE
                            BEGIN
                                INSERT INTO persona(dni, nombre, apellido, correo_electronico, telefono, direccion, estado, fecha_nacimiento)
                                VALUES (@dni, @nombre, @apellido, @correo, @tel, @dir, @estadoPersona, @fnac);
                            END
                        ", cn, tx);

                        cmdPersona.Parameters.AddWithValue("@dni", prc.Dni);
                        cmdPersona.Parameters.AddWithValue("@nombre", (object)prc.oPersona?.Nombre ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@apellido", (object)prc.oPersona?.Apellido ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@correo", (object)prc.oPersona?.CorreoElectronico ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@tel", (object)prc.oPersona?.Telefono ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@dir", (object)prc.oPersona?.Direccion ?? DBNull.Value);
                        cmdPersona.Parameters.Add("@estadoPersona", SqlDbType.Int).Value = (prc.oPersona != null) ? prc.oPersona.Estado : 1;
                        cmdPersona.Parameters.Add("@fnac", SqlDbType.Date).Value = (object)prc.oPersona?.FechaNacimiento ?? DBNull.Value;
                        cmdPersona.ExecuteNonQuery();

                        var cmdRolCliente = new SqlCommand(@"
                            IF NOT EXISTS (SELECT 1 FROM persona_rol_cliente WHERE dni = @dni AND rol_cliente_id = @rolClienteId)
                            BEGIN
                                INSERT INTO persona_rol_cliente(dni, rol_cliente_id, fecha_creacion)
                                VALUES (@dni, @rolClienteId, GETDATE());
                            END
                        ", cn, tx);

                        cmdRolCliente.Parameters.AddWithValue("@dni", prc.Dni);
                        cmdRolCliente.Parameters.AddWithValue("@rolClienteId", prc.oRolCliente.RolClienteId);
                        cmdRolCliente.ExecuteNonQuery();

                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        mensaje = "Error al registrar: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        public bool Actualizar(PersonaRolCliente prc, out string mensaje)
        {
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                // Usamos una transacción porque modificamos una tabla.
                // Si en el futuro necesitaras modificar también la tabla persona_rol_cliente,
                // la transacción ya estaría lista para asegurar que ambas operaciones se completen con éxito.
                var tx = cn.BeginTransaction();

                try
                {
                    // 1) ACTUALIZAR LA TABLA 'PERSONA'
                    var cmdPersona = new SqlCommand(@"
                        UPDATE persona
                           SET nombre             = @nombre,
                               apellido           = @apellido,
                               correo_electronico = @correo,
                               telefono           = @tel,
                               direccion          = @dir,
                               estado             = @estadoPersona,
                               fecha_nacimiento   = @fnac
                         WHERE dni = @dni;", cn, tx);

                    // Asignación de parámetros para la tabla 'persona'
                    cmdPersona.Parameters.AddWithValue("@dni", prc.Dni);
                    // Usamos el objeto oPersona anidado y manejamos los posibles nulos
                    cmdPersona.Parameters.AddWithValue("@nombre", (object)prc.oPersona?.Nombre ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@apellido", (object)prc.oPersona?.Apellido ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@correo", (object)prc.oPersona?.CorreoElectronico ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@tel", (object)prc.oPersona?.Telefono ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@dir", (object)prc.oPersona?.Direccion ?? DBNull.Value);

                    // Manejo del estado (asumiendo que en tu entidad Persona.Estado es un int: 1=activo, 0=inactivo)
                    int estadoPersona = (prc.oPersona != null) ? prc.oPersona.Estado : 1; // Por defecto, activo
                    cmdPersona.Parameters.Add("@estadoPersona", SqlDbType.Int).Value = estadoPersona;

                    // Manejo de fecha de nacimiento nullable
                    var fnac = (prc.oPersona != null ? prc.oPersona.FechaNacimiento : (DateTime?)null);
                    cmdPersona.Parameters.Add("@fnac", SqlDbType.Date).Value = fnac.HasValue ? (object)fnac.Value : DBNull.Value;

                    int filasAfectadas = cmdPersona.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                    {
                        // Si no se actualizó ninguna fila, es porque no se encontró el DNI.
                        throw new Exception("No se encontró la persona con el DNI especificado para actualizar.");
                    }

                    // Si todo salió bien, confirmamos la transacción.
                    tx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Si algo falla, revertimos todos los cambios.
                    try { tx.Rollback(); } catch { /* ignorar errores en el rollback */ }
                    mensaje = "Error al actualizar: " + ex.Message;
                    return false;
                }
            }
        }

        public (int Total, int Activos, int Inactivos) ObtenerEstadisticasBasicas(int rolClienteId)
        {
            int total = 0;
            int activos = 0;
            int inactivos = 0;

            using (var cn = new SqlConnection(Conexion.cadena))
            {
                string query = @"
            SELECT
                COUNT(prc.dni) AS Total,
                ISNULL(SUM(CASE WHEN p.estado = 1 THEN 1 ELSE 0 END), 0) AS Activos,
                ISNULL(SUM(CASE WHEN p.estado = 0 THEN 1 ELSE 0 END), 0) AS Inactivos
            FROM persona_rol_cliente prc
            INNER JOIN persona p ON prc.dni = p.dni
            WHERE prc.rol_cliente_id = @rolClienteId;";

                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@rolClienteId", rolClienteId);
                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            total = Convert.ToInt32(dr["Total"]);
                            activos = Convert.ToInt32(dr["Activos"]);
                            inactivos = Convert.ToInt32(dr["Inactivos"]);
                        }
                    }
                }
            }
            return (total, activos, inactivos);
        }
    }
}
