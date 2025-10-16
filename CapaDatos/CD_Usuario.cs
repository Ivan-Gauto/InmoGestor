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
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                            SELECT 
                                u.dni, u.clave, u.estado, u.fecha_creacion, u.rol_usuario_id,
                                p.nombre, p.apellido, p.correo_electronico, p.direccion, p.telefono,
                                p.fecha_nacimiento, 
                                r.nombre AS rol_nombre
                            FROM usuario u
                            INNER JOIN persona     p ON u.dni = p.dni
                            INNER JOIN rol_usuario r ON u.rol_usuario_id = r.rol_usuario_id;";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario()
                            {
                                Dni = dr["dni"].ToString(),
                                Clave = dr["clave"].ToString(),
                                Estado = Convert.ToBoolean(dr["estado"]),
                                FechaCreacion = Convert.ToDateTime(dr["fecha_creacion"]),
                                RolUsuarioId = Convert.ToInt32(dr["rol_usuario_id"]),
                                oPersona = new Persona()
                                {
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
                catch (Exception)
                {
                    lista = new List<Usuario>();
                }

                return lista;
            }



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

                        // Usuario.Estado es BOOL → INT (0/1)
                        cmdUsuario.Parameters.Add("@estadoUsuario", SqlDbType.Int).Value = u.Estado ? 1 : 0;

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

        public bool Actualizar(Usuario u, out string mensaje)
        {
            mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion.cadena))
            {
                cn.Open();
                var tx = cn.BeginTransaction();

                try
                {
                    // PERSONA
                    var cmdPersona = new SqlCommand(@"
UPDATE persona
   SET nombre            = @nombre,
       apellido          = @apellido,
       correo_electronico= @correo,
       telefono          = @tel,
       direccion         = @dir,
       estado            = @estadoPersona,
       fecha_nacimiento  = @fnac
 WHERE dni = @dni;", cn, tx);

                    cmdPersona.Parameters.AddWithValue("@dni", u.Dni);
                    cmdPersona.Parameters.AddWithValue("@nombre", (object)(u.oPersona != null ? u.oPersona.Nombre : null) ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@apellido", (object)(u.oPersona != null ? u.oPersona.Apellido : null) ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@correo", (object)(u.oPersona != null ? u.oPersona.CorreoElectronico : null) ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@tel", (object)(u.oPersona != null ? u.oPersona.Telefono : null) ?? DBNull.Value);
                    cmdPersona.Parameters.AddWithValue("@dir", (object)(u.oPersona != null ? u.oPersona.Direccion : null) ?? DBNull.Value);

                    int estadoPersona = (u.oPersona != null ? u.oPersona.Estado : 1); // int en BD
                    cmdPersona.Parameters.Add("@estadoPersona", SqlDbType.Int).Value = estadoPersona;

                    cmdPersona.Parameters.Add("@fnac", SqlDbType.Date).Value = u.oPersona.FechaNacimiento.Date;


                    cmdPersona.ExecuteNonQuery();

                    // USUARIO
                    var cmdUsuario = new SqlCommand(@"
UPDATE usuario
   SET clave          = @clave,
       estado         = @estadoUsuario,
       rol_usuario_id = @rol
 WHERE dni = @dni;", cn, tx);

                    cmdUsuario.Parameters.AddWithValue("@dni", u.Dni);
                    cmdUsuario.Parameters.AddWithValue("@clave", u.Clave);
                    cmdUsuario.Parameters.Add("@estadoUsuario", SqlDbType.Int).Value = u.Estado ? 1 : 0;
                    cmdUsuario.Parameters.AddWithValue("@rol", u.RolUsuarioId);

                    int filas = cmdUsuario.ExecuteNonQuery();
                    if (filas == 0)
                    {
                        throw new Exception("No se encontró el usuario a actualizar.");
                    }

                    tx.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547 && ex.Message.Contains("CK_usuario_pass_minlen"))
                        mensaje = "La contraseña debe tener al menos 8 dígitos numéricos.";
                    else
                        mensaje = ex.Message;

                    try { tx.Rollback(); } catch { }
                    return false;
                }

            }
        }


    }
}

