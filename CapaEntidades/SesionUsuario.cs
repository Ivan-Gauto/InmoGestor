using CapaEntidades;

namespace InmoGestor
{
    public static class SesionUsuario
    {
        public static Usuario UsuarioActual { get; private set; }

        public static void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public static void CerrarSesion()
        {
            UsuarioActual = null;
        }

        public static string ObtenerDniUsuarioActual()
        {
            return UsuarioActual?.Dni;
        }

        // Podrías agregar otras propiedades útiles si las necesitás a menudo
        public static string ObtenerNombreCompletoUsuarioActual()
        {
            return UsuarioActual?.NombreCompleto;
        }

        public static int ObtenerRolIdUsuarioActual()
        {
            return UsuarioActual?.RolUsuarioId ?? 0;
        }
    }
}