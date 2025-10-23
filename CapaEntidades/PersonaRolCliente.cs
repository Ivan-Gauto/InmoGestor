using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public class PersonaRolCliente
    {
        public DateTime FechaCreacion { get; set; } = DateTime.Today;
        public string Dni { get; set; } = string.Empty;
        public RolCliente oRolCliente { get; set; }
        public int Estado { get; set; }
        public Persona oPersona { get; set; }
        public string NombreCompleto
        {
            get
            {
                // Devuelve "Apellido, Nombre"
                return $"{oPersona?.Apellido}, {oPersona?.Nombre}";
            }
        }
    }
}
