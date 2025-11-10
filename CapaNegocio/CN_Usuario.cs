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
        private readonly CN_Reportes _cnReportes = new CN_Reportes();

        public List<Usuario> Listar(RolUsuarioFiltro rolFiltro, EstadoFiltro estadoFiltro)
        {
            return _capaDatos.Listar(rolFiltro, estadoFiltro);
        }

        public (int Total, int Admins, int Gerentes, int Operadores, int Activos, int Inactivos) ObtenerEstadisticas()
        {
            return _capaDatos.ObtenerEstadisticas();
        }

        public bool Registrar(Usuario u, string adminEjecutorDni, out string mensaje)
        {
            bool exito = _capaDatos.Registrar(u, out mensaje);

            if (exito)
            {
                try
                {
                    string afectadoDni = u.Dni;
                    string detalle = $"Rol asignado: {u.oRolUsuario.Nombre}";
                    _cnReportes.RegistrarAuditoriaUsuario(adminEjecutorDni, afectadoDni, "Registro de Cuenta", detalle);
                }
                catch (Exception ex)
                {
                    _cnReportes.RegistrarError(null, "CN_Usuario", "Registrar(Log)", ex);
                }
            }
            return exito;
        }

        public bool Actualizar(Usuario u, string dniOriginal, string adminEjecutorDni, out string mensaje)
        {
            bool exito = _capaDatos.Actualizar(u, dniOriginal, out mensaje);

            if (exito)
            {
                try
                {
                    string afectadoDni = u.Dni;
                    string detalle = $"DNI cambiado de {dniOriginal} a {afectadoDni}. Rol: {u.oRolUsuario.Nombre}";
                    _cnReportes.RegistrarAuditoriaUsuario(adminEjecutorDni, afectadoDni, "Actualización de Cuenta", detalle);
                }
                catch (Exception ex)
                {
                    _cnReportes.RegistrarError(null, "CN_Usuario", "Actualizar(Log)", ex);
                }
            }
            return exito;
        }

        public Usuario ValidarUsuario(string dni, string clave)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return null;
            if (string.IsNullOrWhiteSpace(clave))
                return null;

            // Llama al nuevo método de la capa de datos
            return _capaDatos.ValidarUsuario(dni, clave);
        }

        public bool CambiarEstado(string dni, int nuevoEstado, string adminEjecutorDni)
        {
            bool exito = _capaDatos.CambiarEstado(dni, nuevoEstado);

            if (exito)
            {
                try
                {
                    string afectadoDni = dni;
                    string accion = (nuevoEstado == 1) ? "Reactivación de Cuenta" : "Baja Lógica de Cuenta";
                    string detalle = $"Usuario DNI: {afectadoDni}";
                    _cnReportes.RegistrarAuditoriaUsuario(adminEjecutorDni, afectadoDni, accion, detalle);
                }
                catch (Exception ex)
                {
                    _cnReportes.RegistrarError(null, "CN_Usuario", "CambiarEstado(Log)", ex);
                }
            }
            return exito;
        }
    }
}