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
        public List<PersonaRolCliente> ListarInquilinos(int rolClienteId = 2)
        {
            var lista = new List<PersonaRolCliente>();

            using (var cn = new SqlConnection(Conexion.cadena))
            using (var cmd = new SqlCommand(@"
       SELECT  prc.fecha_creacion,
                prc.dni,
                prc.rol_cliente_id,
                p.nombre,
                p.apellido,
                p.correo_electronico,
                p.telefono,
                p.direccion,
                p.estado,
                p.fecha_nacimiento
        FROM persona_rol_cliente prc
        INNER JOIN persona p ON p.dni = prc.dni
        WHERE prc.rol_cliente_id = @rol_cliente_id;", cn))
            {
                cmd.Parameters.AddWithValue("@rol_cliente_id", rolClienteId);
                cn.Open();

                using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
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
            return lista;
        }



    }
}
