using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text; // Para el Registrar

namespace CapaDatos
{
    public class CD_Inmueble
    {
        // --- Listar (Actualizado) ---
        // 1. Modificamos la firma para aceptar un parámetro opcional
        public List<Inmueble> Listar(bool incluirInactivos = false)
        {
            var lista = new List<Inmueble>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                // 2. Modificamos el query para que el WHERE sea dinámico
                string query = @"
            SELECT 
                i.inmueble_id, i.direccion, i.descripcion, i.imagen, i.disponibilidad, i.estado, i.fecha_creacion, i.division,i.disponibilidad
                
                ti.tipo_inmueble_id,
                ti.nombre AS TipoInmuebleNombre,
                
                p.Dni AS PropietarioDni, p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido,
                prc.rol_cliente_id
            FROM dbo.inmueble i
            LEFT JOIN tipo_inmueble ti ON i.id_tipo_inmueble = ti.tipo_inmueble_id 
            LEFT JOIN persona_rol_cliente prc ON i.dni = prc.dni AND i.rol_cliente_id = prc.rol_cliente_id
            LEFT JOIN persona p ON prc.Dni = p.Dni
        "; // Quitamos el WHERE de aquí

                // 3. Añadimos el WHERE dinámicamente
                if (!incluirInactivos)
                {
                    query += " WHERE i.estado = 1"; // El filtro por defecto
                }
                // Si incluirInactivos es true, no añadimos el WHERE, trayendo todo.

                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                // 4. Implementamos el control de DBNull.Value
                                IdInmueble = Convert.ToInt32(dr["inmueble_id"]),
                                Direccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString(),
                                Descripcion = dr["descripcion"] == DBNull.Value ? "" : dr["descripcion"].ToString(),
                                RutaImagen = dr["imagen"] == DBNull.Value ? "" : dr["imagen"].ToString(),
                                Disponibilidad = dr["disponibilidad"] == DBNull.Value ? 0 : Convert.ToInt32(dr["disponibilidad"]),
                                Estado = dr["estado"] == DBNull.Value ? 0 : Convert.ToInt32(dr["estado"]),
                                FechaCreacion = Convert.ToDateTime(dr["fecha_creacion"]),
                                Division = dr["division"] == DBNull.Value ? "" : dr["division"].ToString(),

                                oTipoInmueble = dr["tipo_inmueble_id"] == DBNull.Value ? new TipoInmueble() : new TipoInmueble
                                {
                                    IdTipoInmueble = Convert.ToInt32(dr["tipo_inmueble_id"]),
                                    Nombre = dr["TipoInmuebleNombre"].ToString()
                                },

                                oPropietario = dr["PropietarioDni"] == DBNull.Value ? new PersonaRolCliente { oPersona = new Persona() } : new PersonaRolCliente
                                {
                                    Dni = dr["PropietarioDni"].ToString(),
                                    oRolCliente = new RolCliente { RolClienteId = Convert.ToInt32(dr["rol_cliente_id"]) },
                                    oPersona = new Persona
                                    {
                                        Nombre = dr["PropietarioNombre"].ToString(),
                                        Apellido = dr["PropietarioApellido"].ToString()
                                    }
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar inmuebles: " + ex.Message);
                    lista = new List<Inmueble>();
                }
            }
            return lista;
        }

        // --- Registrar (Nuevo) ---
        public bool Registrar(Inmueble obj, out string Mensaje)
        {
            bool exito = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("INSERT INTO dbo.inmueble (");
                    query.AppendLine("    direccion, descripcion, estado, fecha_creacion, id_tipo_inmueble, ");
                    query.AppendLine("    dni, rol_cliente_id, imagen, disponibilidad, division");
                    query.AppendLine(") VALUES (");
                    query.AppendLine("    @direccion, @descripcion, @estado, GETDATE(), @id_tipo_inmueble, ");
                    query.AppendLine("    @dni, @rol_cliente_id, @imagen, @disponibilidad, @division");
                    query.AppendLine(");");

                    SqlCommand cmd = new SqlCommand(query.ToString(), cn);
                    cmd.Parameters.AddWithValue("@direccion", obj.Direccion);
                    cmd.Parameters.AddWithValue("@descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@estado", obj.Estado); // 1=Activo (Baja lógica)
                    cmd.Parameters.AddWithValue("@id_tipo_inmueble", obj.oTipoInmueble.IdTipoInmueble);
                    cmd.Parameters.AddWithValue("@dni", obj.oPropietario.Dni);
                    cmd.Parameters.AddWithValue("@rol_cliente_id", obj.oPropietario.oRolCliente.RolClienteId);
                    cmd.Parameters.AddWithValue("@disponibilidad", obj.Disponibilidad);

                    // Manejamos valores que pueden ser NULL
                    cmd.Parameters.AddWithValue("@imagen", string.IsNullOrEmpty(obj.RutaImagen) ? (object)DBNull.Value : obj.RutaImagen);
                    cmd.Parameters.AddWithValue("@division", string.IsNullOrEmpty(obj.Division) ? (object)DBNull.Value : obj.Division);

                    cmd.CommandType = CommandType.Text;
                    cn.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        exito = true;
                    }
                    else
                    {
                        Mensaje = "No se pudo registrar el inmueble.";
                    }
                }
            }
            catch (Exception ex)
            {
                exito = false;
                Mensaje = "Error al registrar inmueble: " + ex.Message;
            }

            return exito;
        }

        public List<TipoInmueble> ListarTiposInmueble()
        {
            var lista = new List<TipoInmueble>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                // --- CORREGIDO ---
                // Se usa el nombre de tu tabla (tipo_inmueble)
                // y tus columnas (tipo_inmueble_id, nombre).
                // Se quita el "WHERE Estado = 1" porque tu tabla no lo tiene.
                string query = "SELECT tipo_inmueble_id, nombre FROM tipo_inmueble";

                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new TipoInmueble
                            {
                                // --- CORREGIDO ---
                                // Se mapean los nombres de tus columnas
                                IdTipoInmueble = Convert.ToInt32(dr["tipo_inmueble_id"]),
                                Nombre = dr["nombre"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar tipos de inmueble: " + ex.Message);
                    lista = new List<TipoInmueble>();
                }
            }
            return lista;
        }

        public bool DesactivarLogico(int idInmueble)
        {
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                string query = "UPDATE dbo.inmueble SET estado = 0 WHERE inmueble_id = @idInmueble";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@idInmueble", idInmueble);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al desactivar inmueble: " + ex.Message);
                    return false;
                }
            }
        }

        public bool ReactivarLogico(int idInmueble)
        {
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                string query = "UPDATE dbo.inmueble SET estado = 1 WHERE inmueble_id = @idInmueble";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@idInmueble", idInmueble);
                    cmd.CommandType = CommandType.Text;
                    cn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al reactivar inmueble: " + ex.Message);
                    return false;
                }
            }
        }

        // --- Actualizar (Editar) ---
        public bool Actualizar(Inmueble obj, out string Mensaje)
        {
            bool exito = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("UPDATE dbo.inmueble SET");
                    query.AppendLine("    direccion = @direccion,");
                    query.AppendLine("    descripcion = @descripcion,");
                    query.AppendLine("    id_tipo_inmueble = @id_tipo_inmueble,");
                    query.AppendLine("    dni = @dni,");
                    query.AppendLine("    rol_cliente_id = @rol_cliente_id,");
                    query.AppendLine("    imagen = @imagen,");
                    query.AppendLine("    disponibilidad = @disponibilidad,");
                    query.AppendLine("    division = @division");
                    query.AppendLine("WHERE inmueble_id = @inmueble_id;");

                    SqlCommand cmd = new SqlCommand(query.ToString(), cn);
                    cmd.Parameters.AddWithValue("@direccion", obj.Direccion);
                    cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(obj.Descripcion) ? (object)DBNull.Value : obj.Descripcion);
                    cmd.Parameters.AddWithValue("@id_tipo_inmueble", obj.oTipoInmueble?.IdTipoInmueble ?? 0);
                    cmd.Parameters.AddWithValue("@dni", string.IsNullOrEmpty(obj.oPropietario?.Dni) ? (object)DBNull.Value : obj.oPropietario.Dni);
                    cmd.Parameters.AddWithValue("@rol_cliente_id", obj.oPropietario?.oRolCliente?.RolClienteId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@disponibilidad", obj.Disponibilidad);
                    cmd.Parameters.AddWithValue("@imagen", string.IsNullOrEmpty(obj.RutaImagen) ? (object)DBNull.Value : obj.RutaImagen);
                    cmd.Parameters.AddWithValue("@division", string.IsNullOrEmpty(obj.Division) ? (object)DBNull.Value : obj.Division);
                    cmd.Parameters.AddWithValue("@inmueble_id", obj.IdInmueble);

                    cmd.CommandType = CommandType.Text;
                    cn.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        exito = true;
                    }
                    else
                    {
                        Mensaje = "No se pudo actualizar el inmueble.";
                    }
                }
            }
            catch (Exception ex)
            {
                exito = false;
                Mensaje = "Error al actualizar inmueble: " + ex.Message;
            }

            return exito;
        }

        public List<Inmueble> ListarDisponibles()
        {
            List<Inmueble> lista = new List<Inmueble>();

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                string query = @"
                    SELECT inmueble_id, direccion, estado, disponibilidad
                    FROM inmueble
                    WHERE estado = 1 AND disponibilidad = 1;";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Inmueble obj = new Inmueble
                        {
                            IdInmueble = Convert.ToInt32(dr["inmueble_id"]),
                            Direccion = dr["direccion"] != DBNull.Value ? dr["direccion"].ToString() : string.Empty,
                            Estado = Convert.ToInt32(dr["estado"]),
                            Disponibilidad = Convert.ToInt32(dr["disponibilidad"])
                        };
                        lista.Add(obj);
                    }
                }
            }

            return lista;
        }

    }
}