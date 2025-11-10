using System;
using System.Data;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Reportes
    {
        private CD_Reportes objCD_Reportes = new CD_Reportes();

        public DataTable ObtenerRendimientoOperadores(DateTime fechaInicio, DateTime fechaFin)
        {
            return objCD_Reportes.ObtenerRendimientoOperadores(fechaInicio, fechaFin);
        }

        public DataTable ObtenerReporteOcupacion()
        {
            return objCD_Reportes.ObtenerReporteOcupacion();
        }
    }
}