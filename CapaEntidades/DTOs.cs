using System;

namespace CapaEntidades
{
    public class ContratoGridRow
    {
        public int Id { get; set; }
        public string Inquilino { get; set; }          // "Apellido, Nombre"
        public string Direccion { get; set; }          // Dirección del inmueble
        public string Inmueble { get; set; }           // Descripción / alias
        public decimal PrecioCuota { get; set; }
        public int CantCuotas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MoraDiaria { get; set; }        // % diario (calculado desde mensual)
        public int Estado { get; set; }                // por si querés pintar estado
    }
}
