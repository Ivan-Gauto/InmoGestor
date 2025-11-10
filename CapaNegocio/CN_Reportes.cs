using System;
using System.Data;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidades;

namespace CapaNegocio
{
    public class CN_Reportes
    {
        private CD_Reportes objCD_Reportes = new CD_Reportes();

        public DataTable ObtenerLogErrorAgrupado(DateTime fechaInicio, DateTime fechaFin)
        {
            return objCD_Reportes.ObtenerLogErrorAgrupado(fechaInicio, fechaFin);
        }

        public List<LogUsuario> ListarAuditoriaUsuarios(string adminDni, DateTime inicio, DateTime fin)
        {
            return objCD_Reportes.ListarAuditoriaUsuarios(adminDni, inicio, fin);
        }

        public DataTable ObtenerRendimientoOperadores(DateTime fechaInicio, DateTime fechaFin)
        {
            return objCD_Reportes.ObtenerRendimientoOperadores(fechaInicio, fechaFin);
        }

        public DataTable ObtenerReporteOcupacion()
        {
            return objCD_Reportes.ObtenerReporteOcupacion();
        }

        public void RegistrarError(int? usuarioId, string formulario, string metodo, Exception ex)
        {
            objCD_Reportes.RegistrarError(usuarioId, formulario, metodo, ex.Message, ex.StackTrace);
        }

        public void RegistrarAuditoriaUsuario(string adminDni, string afectadoDni, string accion, string detalle)
        {
            objCD_Reportes.RegistrarAuditoriaUsuario(adminDni, afectadoDni, accion, detalle);
        }

        public DataTable ObtenerInquilinosMorosos(DateTime desde, DateTime hasta, string dniInquilino, int? contratoId)
        {
            return objCD_Reportes.ObtenerInquilinosMorosos(desde, hasta, dniInquilino, contratoId);
        }

    }
}