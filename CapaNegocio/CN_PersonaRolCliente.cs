using CapaDatos;
using CapaEntidades;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_PersonaRolCliente
    {
        private readonly CD_PersonaRolCliente _capaDatos = new CD_PersonaRolCliente();

        //Inquilinos

        public List<PersonaRolCliente> ListarInquilinosActivos()
        {
            return _capaDatos.Listar(2, true);
        }

        public List<PersonaRolCliente> ListarTodosLosInquilinos()
        {
            return _capaDatos.Listar(2, null);
        }

        public (int Total, int Activos, int Inactivos) ObtenerEstadisticasInquilinos()
        {
            return _capaDatos.ObtenerEstadisticasBasicas(2); // ID de Inquilino
        }

        //Propietarios

        public List<PersonaRolCliente> ListarPropietarios()
        {
            return _capaDatos.Listar(1);
        }

        public (int Total, int Activos, int Inactivos) ObtenerEstadisticasPropietarios()
        {
            return _capaDatos.ObtenerEstadisticasBasicas(1); // ID de Propietario
        }

        //Metodos comunes

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

        public bool CambiarEstado(string dni, int nuevoEstado) =>
            _capaDatos.CambiarEstado(dni, nuevoEstado);
    }
}
