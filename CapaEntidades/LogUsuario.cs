using System;

namespace CapaEntidades
{
    public class LogUsuario
    {
        public long LogUsuarioId { get; set; }
        public DateTime FechaHora { get; set; }
        public string AdminEjecutorDni { get; set; }
        public string UsuarioAfectadoDni { get; set; }
        public string Accion { get; set; }
        public string Detalle { get; set; }

        // Opcional: Podrías agregar propiedades para los nombres si los traes con JOIN en el futuro
        // public string NombreAdminEjecutor { get; set; }
        // public string NombreUsuarioAfectado { get; set; }
    }
}