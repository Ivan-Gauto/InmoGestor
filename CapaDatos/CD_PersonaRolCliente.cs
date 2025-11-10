using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CapaEntidades;

namespace CapaDatos
{
    public class CD_PersonaRolCliente
    {
        public List<PersonaRolCliente> Listar(TipoRolCliente rol, EstadoFiltro filtro)
        {
            var lista = new List<PersonaRolCliente>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                var queryBuilder = new StringBuilder(@"
                SELECT prc.fecha_creacion, prc.dni, prc.rol_cliente_id,
                       prc.estado, -- <-- MODIFICADO: Añadido el estado del rol
                       p.nombre, p.apellido, p.correo_electronico, p.telefono,
                       p.direccion, p.estado AS EstadoPersona, p.fecha_nacimiento -- <-- MODIFICADO: Renombrado el estado de la persona
                FROM persona_rol_cliente prc
                INNER JOIN persona p ON p.dni = prc.dni
                WHERE prc.rol_cliente_id = @rol_cliente_id");

                switch (filtro)
                {
                    case EstadoFiltro.Activos:
                        queryBuilder.Append(" AND prc.estado = 1"); // <-- MODIFICADO: Filtra por prc.estado
                        break;
                    case EstadoFiltro.Inactivos:
                        queryBuilder.Append(" AND prc.estado = 0"); // <-- MODIFICADO: Filtra por prc.estado
                        break;
                    case EstadoFiltro.Morosos:
                        queryBuilder.Append(" AND EXISTS (SELECT 1 FROM Cuotas c WHERE c.DniInquilino = prc.dni AND c.Pagada = 0 AND c.FechaVencimiento < GETDATE())");
                        break;
                }

                using (var cmd = new SqlCommand(queryBuilder.ToString(), cn))
                {
                    cmd.Parameters.AddWithValue("@rol_cliente_id", (int)rol);
                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PersonaRolCliente
                            {
                                FechaCreacion = dr.GetDateTime(dr.GetOrdinal("fecha_creacion")),
                                Dni = dr["dni"]?.ToString() ?? "",
                                Estado = Convert.ToInt32(dr["estado"]), // <-- MODIFICADO: Lee prc.estado
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
                                    Estado = Convert.ToInt32(dr["EstadoPersona"]), // <-- MODIFICADO: Lee p.estado (renombrado)
                                    FechaNacimiento = dr.GetDateTime(dr.GetOrdinal("fecha_nacimiento"))
                                }
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public (int Total, int Activos, int Inactivos) ObtenerEstadisticasBasicas(TipoRolCliente rol)
        {
            int total = 0;
            int activos = 0;
            int inactivos = 0;

            using (var cn = new SqlConnection(Conexion.cadena))
            {
                // <-- MODIFICADO: Los CASE WHEN ahora usan prc.estado
                string query = @"
                SELECT
                    COUNT(prc.dni) AS Total,
                    ISNULL(SUM(CASE WHEN prc.estado = 1 THEN 1 ELSE 0 END), 0) AS Activos,
                    ISNULL(SUM(CASE WHEN prc.estado = 0 THEN 1 ELSE 0 END), 0) AS Inactivos
                FROM persona_rol_cliente prc
                INNER JOIN persona p ON prc.dni = p.dni
                WHERE prc.rol_cliente_id = @rolClienteId;";

                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@rolClienteId", (int)rol);
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

        public bool CambiarEstado(string dni, int nuevoEstado, int idRolCliente)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                {
                    // <-- MODIFICADO: Corregido nombre de columna (IdRolCliente -> rol_cliente_id)
                    string query = "UPDATE persona_rol_cliente SET Estado = @estado WHERE Dni = @dni AND rol_cliente_id = @idRolCliente";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@idRolCliente", idRolCliente);
                    cmd.CommandType = CommandType.Text;

                    cn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
            catch (Exception)
            {
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
                        // Este comando (Insert/Update en persona) está bien como estaba.
                        // Maneja el estado general de la *persona* en el sistema.
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

                        // <-- MODIFICADO: Añadido el campo 'estado' al INSERT, con valor 1 (Activo)
                        var cmdRolCliente = new SqlCommand(@"
                            IF NOT EXISTS (SELECT 1 FROM persona_rol_cliente WHERE dni = @dni AND rol_cliente_id = @rolClienteId)
                            BEGIN
                                INSERT INTO persona_rol_cliente(dni, rol_cliente_id, fecha_creacion, estado)
                                VALUES (@dni, @rolClienteId, GETDATE(), 1);
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

        public bool Actualizar(string dniOriginal, PersonaRolCliente prc, out string mensaje)
        {
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                var tx = cn.BeginTransaction();

                try
                {
                    var cmdPersona = new SqlCommand(@"
                UPDATE persona
                   SET dni = @dniNuevo,
                       nombre = @nombre,
                       apellido = @apellido,
                       correo_electronico = @correo,
                       telefono = @tel,
                       direccion = @dir,
                       estado = @estadoPersona,
                       fecha_nacimiento = @fnac
                 WHERE dni = @dniOriginal;", cn, tx);

                    cmdPersona.Parameters.AddWithValue("@dniNuevo", prc.oPersona.Dni);
                    cmdPersona.Parameters.AddWithValue("@nombre", (object)prc.oPersona?.Nombre ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@apellido", (object)prc.oPersona?.Apellido ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@correo", (object)prc.oPersona?.CorreoElectronico ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@tel", (object)prc.oPersona?.Telefono ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@dir", (object)prc.oPersona?.Direccion ?? DBNull.Value);

                    int estadoPersona = (prc.oPersona != null) ? prc.oPersona.Estado : 1;
                    cmdPersona.Parameters.Add("@estadoPersona", SqlDbType.Int).Value = estadoPersona;

                    var fnac = (prc.oPersona != null ? prc.oPersona.FechaNacimiento : (DateTime?)null);
                    cmdPersona.Parameters.Add("@fnac", SqlDbType.Date).Value = fnac.HasValue ? (object)fnac.Value : DBNull.Value;

                    cmdPersona.Parameters.AddWithValue("@dniOriginal", dniOriginal);

                    int filasAfectadas = cmdPersona.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                    {
                        throw new Exception("No se encontró la persona con el DNI original para actualizar.");
                    }

                    tx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    mensaje = "Error al actualizar: " + ex.Message;
                    return false;
                }
            }
        }
    }
}