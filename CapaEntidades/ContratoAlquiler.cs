using System;

namespace CapaEntidades
{
    public class ContratoAlquiler
    {
        // --- PK
        public int ContratoId { get; set; }

        // --- Datos base
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Condiciones { get; set; } = string.Empty;
        public int CantidadCuotas { get; set; }
        public decimal PrecioCuota { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // --- FKs (IDs reales)
        public int InmuebleId { get; set; }
        public string DniInquilino { get; set; } = string.Empty;
        public int RolInquilinoId { get; set; }
        public decimal TasaMoraMensual { get; set; }
        public int Estado { get; set; }
        public int UsuarioCreador { get; set; }

        // --- Objetos relacionados (inicializados)
        public Inmueble oInmueble { get; set; } = new Inmueble();
        public Persona oInquilino { get; set; } = new Persona();
        public PersonaRolCliente oInquilinoRol { get; set; } = new PersonaRolCliente();
        public Usuario oUsuarioCreadorObj { get; set; } = new Usuario();

        // --- Propiedades derivadas (no mapeadas)
        public string InquilinoNombreCompleto => $"{oInquilino.Nombre} {oInquilino.Apellido}";
        public string DireccionInmueble => oInmueble.Direccion;
        public string EstadoTexto
        {
            get
            {
                switch (Estado)
                {
                    case 0: return "Anulado";
                    case 1: return "Vigente";
                    case 2: return "Finalizado";
                    default: return "Desconocido";
                }
            }
        }
    }
}
