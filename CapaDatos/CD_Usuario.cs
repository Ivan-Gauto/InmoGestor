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
    public class CD_Usuario
    {
        public List<Usuario> Listar(RolUsuarioFiltro rolFiltro, EstadoFiltro estadoFiltro)
        {
            var lista = new List<Usuario>();
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                var queryBuilder = new StringBuilder(@"
            SELECT u.dni, u.clave, u.estado, u.fecha_creacion, u.rol_usuario_id,
                   p.nombre, p.apellido, p.correo_electronico, p.direccion, p.telefono,
                   p.fecha_nacimiento, r.nombre AS rol_nombre
            FROM usuario u
            INNER JOIN persona p ON u.dni = p.dni
            INNER JOIN rol_usuario r ON u.rol_usuario_id = r.rol_usuario_id");

                var conditions = new List<string>();
                var cmd = new SqlCommand();

                if (rolFiltro != RolUsuarioFiltro.Todos)
                {
                    conditions.Add("u.rol_usuario_id = @rolId");
                    int rolId = 0;
                    if (rolFiltro == RolUsuarioFiltro.Administradores) rolId = 1;
                    else if (rolFiltro == RolUsuarioFiltro.Gerentes) rolId = 2;
                    else if (rolFiltro == RolUsuarioFiltro.Operadores) rolId = 3;
                    cmd.Parameters.AddWithValue("@rolId", rolId);
                }

                if (estadoFiltro != EstadoFiltro.Todos)
                {
                    if (estadoFiltro == EstadoFiltro.Activos)
                    {
                        conditions.Add("u.estado = 1");
                    }
                    else if (estadoFiltro == EstadoFiltro.Inactivos)
                    {
                        conditions.Add("u.estado = 0");
                    }
                }

                if (conditions.Count > 0)
                {
                    queryBuilder.Append(" WHERE " + string.Join(" AND ", conditions));
                }

                cmd.CommandText = queryBuilder.ToString();
                cmd.Connection = cn;

                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Usuario()
                        {
                            Dni = dr["dni"].ToString(),
                            Clave = dr["clave"].ToString(),
                            Estado = Convert.ToInt32(dr["estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["fecha_creacion"]),
                            RolUsuarioId = Convert.ToInt32(dr["rol_usuario_id"]),
                            oPersona = new Persona()
                            {
                                Dni = dr["dni"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                CorreoElectronico = dr["correo_electronico"].ToString(),
                                Direccion = dr["direccion"].ToString(),
                                Telefono = dr["telefono"].ToString(),
                                FechaNacimiento = Convert.ToDateTime(dr["fecha_nacimiento"])
                            },
                            oRolUsuario = new RolUsuario()
                            {
                                RolUsuarioId = Convert.ToInt32(dr["rol_usuario_id"]),
                                Nombre = dr["rol_nombre"].ToString()
                            }
                        });
                    }
                }
            }
            return lista;
        }

        public (int Total, int Admins, int Gerentes, int Operadores, int Activos, int Inactivos) ObtenerEstadisticas()
        {
            int total = 0, admins = 0, gerentes = 0, operadores = 0, activos = 0, inactivos = 0;
            using (var cn = new SqlConnection(Conexion.cadena))
            {
                string query = @"
            SELECT
                COUNT(*) AS Total,
                ISNULL(SUM(CASE WHEN rol_usuario_id = 1 THEN 1 ELSE 0 END), 0) AS Admins,
                ISNULL(SUM(CASE WHEN rol_usuario_id = 2 THEN 1 ELSE 0 END), 0) AS Gerentes,
                ISNULL(SUM(CASE WHEN rol_usuario_id = 3 THEN 1 ELSE 0 END), 0) AS Operadores,
                ISNULL(SUM(CASE WHEN estado = 1 THEN 1 ELSE 0 END), 0) AS Activos,
                ISNULL(SUM(CASE WHEN estado = 0 THEN 1 ELSE 0 END), 0) AS Inactivos
            FROM usuario;";

                using (var cmd = new SqlCommand(query, cn))
                {
                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            total = Convert.ToInt32(dr["Total"]);
                            admins = Convert.ToInt32(dr["Admins"]);
                            gerentes = Convert.ToInt32(dr["Gerentes"]);
                            operadores = Convert.ToInt32(dr["Operadores"]);
                            activos = Convert.ToInt32(dr["Activos"]);
                            inactivos = Convert.ToInt32(dr["Inactivos"]);
                        }
                    }
                }
            }
            return (total, admins, gerentes, operadores, activos, inactivos);
        }


        // alta (upsert persona + insert usuario)
        public bool Registrar(Usuario u, out string mensaje)
        {
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) UPSERT de persona
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

                        cmdPersona.CommandType = CommandType.Text;

                        cmdPersona.Parameters.AddWithValue("@dni", u.Dni ?? string.Empty);
                        cmdPersona.Parameters.AddWithValue("@nombre", (object)u.oPersona?.Nombre ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@apellido", (object)u.oPersona?.Apellido ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@correo", (object)u.oPersona?.CorreoElectronico ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@tel", (object)u.oPersona?.Telefono ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@dir", (object)u.oPersona?.Direccion ?? DBNull.Value);

                        // Persona.Estado es INT (0/1) → asegurar entero 0/1
                        int estadoPersona = 1; // por defecto activo
                        if (u.oPersona != null)
                        {
                            // si tu clase Persona.Estado = int (0/1):
                            estadoPersona = (u.oPersona.Estado != 0) ? 1 : 0;

                            // si fuera bool, usarías: estadoPersona = u.oPersona.Estado ? 1 : 0;
                        }
                        cmdPersona.Parameters.Add("@estadoPersona", SqlDbType.Int).Value = estadoPersona;

                        // Fecha obligatoria:
                        cmdPersona.Parameters.Add("@fnac", SqlDbType.Date).Value = u.oPersona.FechaNacimiento.Date;


                        cmdPersona.ExecuteNonQuery();

                        // 2) INSERT de usuario (dni es PK; si existe, aborta)
                        var cmdUsuario = new SqlCommand(@"
                        IF EXISTS (SELECT 1 FROM usuario WHERE dni=@dni)
                        BEGIN
                            RAISERROR('Ya existe un usuario con ese DNI.', 16, 1);
                        END
                        ELSE
                        BEGIN
                            INSERT INTO usuario(dni, clave, estado, fecha_creacion, rol_usuario_id)
                            VALUES (@dni, @clave, @estadoUsuario, GETDATE(), @rol);
                        END
                        ", cn, tx);

                        cmdUsuario.CommandType = CommandType.Text;

                        cmdUsuario.Parameters.AddWithValue("@dni", u.Dni ?? string.Empty);
                        cmdUsuario.Parameters.AddWithValue("@clave", u.Clave ?? string.Empty);

                        cmdUsuario.Parameters.AddWithValue("@estadoUsuario", u.Estado);

                        cmdUsuario.Parameters.AddWithValue("@rol", u.RolUsuarioId);

                        cmdUsuario.ExecuteNonQuery();

                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { /* ignore */ }
                        mensaje = ex.Message;
                        return false;
                    }
                }
            }
        }

        public bool CambiarEstado(string dni, int nuevoEstado)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(Conexion.cadena))
                {
                    string query = "UPDATE usuario SET estado = @estado WHERE dni = @dni";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@dni", dni);
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

        public bool Actualizar(Usuario u, string dniOriginal, out string mensaje)
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
                    UPDATE persona
                    SET dni                  = @dniNuevo,
                        nombre               = @nombre,
                        apellido             = @apellido,
                        correo_electronico   = @correo,
                        telefono             = @telefono,
                        direccion            = @direccion,
                        fecha_nacimiento     = @fechaNacimiento
                    WHERE dni = @dniActual;
                ", cn, tx);

                        cmdPersona.Parameters.AddWithValue("@dniActual", dniOriginal);
                        cmdPersona.Parameters.AddWithValue("@dniNuevo", u.Dni ?? string.Empty);
                        cmdPersona.Parameters.AddWithValue("@nombre", (object)u.oPersona.Nombre ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@apellido", (object)u.oPersona.Apellido ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@correo", (object)u.oPersona.CorreoElectronico ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@telefono", (object)u.oPersona.Telefono ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@direccion", (object)u.oPersona.Direccion ?? DBNull.Value);
                        cmdPersona.Parameters.AddWithValue("@fechaNacimiento", u.oPersona.FechaNacimiento.Date);
                        cmdPersona.CommandType = CommandType.Text;
                        cmdPersona.ExecuteNonQuery();


                        // 2. Actualizar la tabla USUARIO (lo que NO está en cascada, como la clave y el rol)
                        var cmdUsuario = new SqlCommand(@"
                    UPDATE usuario
                    SET clave = @clave,
                        estado = @estado,
                        rol_usuario_id = @rolId
                    WHERE dni = @dniNuevo; 
                    -- Usamos @dniNuevo porque el DNI ya se actualizó en el paso 1 (o es el mismo)
                ", cn, tx);

                        cmdUsuario.Parameters.AddWithValue("@clave", u.Clave ?? string.Empty);
                        cmdUsuario.Parameters.AddWithValue("@estado", u.Estado);
                        cmdUsuario.Parameters.AddWithValue("@rolId", u.RolUsuarioId);
                        cmdUsuario.Parameters.AddWithValue("@dniNuevo", u.Dni ?? string.Empty);
                        cmdUsuario.CommandType = CommandType.Text;
                        cmdUsuario.ExecuteNonQuery();

                        // 3. Confirmar todos los cambios
                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { /* Ignore */ }
                        mensaje = "Error al actualizar (BD): " + ex.Message;
                        return false;
                    }
                }
            }
        }


    }
}

