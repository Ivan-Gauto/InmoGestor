using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidades;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        public CD_Usuario _capaDatos = new CD_Usuario();

        public List<Usuario> Listar(RolUsuarioFiltro rolFiltro, EstadoFiltro estadoFiltro)
        {
            return _capaDatos.Listar(rolFiltro, estadoFiltro);
        }

        public (int Total, int Admins, int Gerentes, int Operadores, int Activos, int Inactivos) ObtenerEstadisticas()
        {
            return _capaDatos.ObtenerEstadisticas();
        }

        public bool Registrar(Usuario u, string adminEjecutorDni, out string mensaje) // <--- Acepta STRING
        {
            bool exito = _capaDatos.Registrar(u, out mensaje);

            if (exito)
            {
                try
                {
                    string afectadoDni = u.Dni;
                    string detalle = $"Rol asignado: {u.oRolUsuario.Nombre}"; // Asegúrate que oRolUsuario.Nombre se cargue bien
                    new CN_Log().RegistrarAuditoriaUsuario(adminEjecutorDni, afectadoDni, "Creación de Cuenta", detalle); // <--- Pasa STRINGS
                }
                catch { }
            }
            return exito;
        }

        public bool Actualizar(Usuario u, string dniOriginal, string adminEjecutorDni, out string mensaje) // <--- Acepta STRING
        {
            bool exito = _capaDatos.Actualizar(u, dniOriginal, out mensaje);

            if (exito)
            {
                try
                {
                    string afectadoDni = u.Dni; // El DNI nuevo
                    string detalle = $"DNI cambiado de {dniOriginal} a {afectadoDni}. Rol: {u.oRolUsuario.Nombre}"; // Asegúrate que oRolUsuario.Nombre se cargue
                    new CN_Log().RegistrarAuditoriaUsuario(adminEjecutorDni, afectadoDni, "Actualización de Cuenta", detalle); // <--- Pasa STRINGS
                }
                catch { }
            }
            return exito;
        }

        public bool CambiarEstado(string dni, int nuevoEstado, string adminEjecutorDni) // <--- Acepta STRING
        {
            bool exito = _capaDatos.CambiarEstado(dni, nuevoEstado);

            if (exito)
            {
                try
                {
                    string afectadoDni = dni;
                    string accion = (nuevoEstado == 1) ? "Reactivación de Cuenta" : "Baja Lógica de Cuenta";
                    string detalle = $"Usuario DNI: {afectadoDni}";

                    new CN_Log().RegistrarAuditoriaUsuario(adminEjecutorDni, afectadoDni, accion, detalle); // <--- Pasa STRINGS
                }
                catch { }
            }
            return exito;
        }
    }
}