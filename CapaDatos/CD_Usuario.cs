using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;


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
                    string query = "SELECT u.dni, u.clave, u.estado, u.fecha_creacion, u.rol_usuario_id, " +
                                   "p.nombre, p.apellido, p.correo_electronico, " +
                                   "r.nombre AS rol_nombre " +
                                   "FROM usuario u " +
                                   "INNER JOIN persona p ON u.dni = p.dni " +
                                   "INNER JOIN rol_usuario r ON u.rol_usuario_id = r.rol_usuario_id";
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
                                    CorreoElectronico = dr["correo_electronico"].ToString()
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
    }
}