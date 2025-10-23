using CapaDatos;
using CapaEntidades;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_PersonaRolCliente
    {
        private readonly CD_PersonaRolCliente _capaDatos = new CD_PersonaRolCliente();

        public List<PersonaRolCliente> ListarClientes(TipoRolCliente rol, EstadoFiltro filtro)
        {
            return _capaDatos.Listar(rol, filtro);
        }

        public (int Total, int Activos, int Inactivos) ObtenerEstadisticas(TipoRolCliente rol)
        {
            return _capaDatos.ObtenerEstadisticasBasicas(rol);
        }

        public bool Registrar(PersonaRolCliente prc, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(prc.oPersona?.Nombre))
            {
                mensaje = "El nombre no puede estar vacío.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(prc.Dni))
            {
                mensaje = "El DNI no puede estar vacío.";
                return false;
            }

            return _capaDatos.Registrar(prc, out mensaje);
        }

        public bool Actualizar(PersonaRolCliente prc, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(prc.Dni))
            {
                mensaje = "El DNI es necesario para actualizar.";
                return false;
            }

            return _capaDatos.Actualizar(prc, out mensaje);
        }

        public bool CambiarEstado(string dni, int nuevoEstado, int idRolCliente)
        {
            return _capaDatos.CambiarEstado(dni, nuevoEstado, idRolCliente);
        }
    }
}