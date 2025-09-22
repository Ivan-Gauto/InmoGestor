using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CapaEntidad
{
    public class Usuario
    {
        // PK (en BD: PRIMARY KEY (dni))
        public string Dni { get; set; } = string.Empty;

        // Credenciales / estado
        public string Clave { get; set; } = string.Empty;   // ideal: guardar hash
        public bool Estado { get; set; }                     // 0 = Inactivo, 1 = Activo
        public DateTime FechaCreacion { get; set; }

        // FK a rol_usuario
        public int RolUsuarioId { get; set; }
        public RolUsuario oRolUsuario { get; set; }

        // Navegación a persona (mismo dni)
        public Persona oPersona { get; set; }

        // Props de conveniencia (no mapeadas)
        public string NombreCompleto => $"{oPersona.Nombre} {oPersona.Apellido}";
        public string Correo => oPersona.CorreoElectronico;
        public bool Activo => Estado; // atajo para la UI

    }
}
