using CapaDatos;
using CapaEntidades;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_Log
    {
        private readonly CD_Log _capaDatos = new CD_Log();

        public void RegistrarAuditoriaUsuario(string adminEjecutorDni, string usuarioAfectadoDni, string accion, string detalle)
        {
            try
            {
                _capaDatos.RegistrarAuditoriaUsuario(adminEjecutorDni, usuarioAfectadoDni, accion, detalle);
            }
            catch (Exception ex)
            {
                // Considera manejar o registrar esta excepción si falla la auditoría
            }
        }

        public List<LogUsuario> ListarAuditoriaUsuarios(string adminDni, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return _capaDatos.ListarAuditoriaUsuarios(adminDni, fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                // Considera registrar este error en log_error
                return new List<LogUsuario>(); // Devolver lista vacía en caso de error
            }
        }

        public void RegistrarError(string usuarioDni, string formulario, string metodo, string mensajeError, string stackTrace = null)
        {
            try
            {
                _capaDatos.RegistrarError(usuarioDni, formulario, metodo, mensajeError, stackTrace);
            }
            catch
            {
                // Si falla el log de errores, no hay mucho más que hacer
            }
        }
    }
}