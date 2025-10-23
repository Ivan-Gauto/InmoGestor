using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidades;

namespace CapaNegocio
{
    public class CN_Usuario
    {

        public CD_Usuario _capaDatos = new CD_Usuario();

        public List<Usuario> Listar(RolUsuarioFiltro rolFiltro, EstadoFiltro estadoFiltro)
        {
            return _capaDatos.Listar(rolFiltro, estadoFiltro);
        }

        public (int Total, int Admins, int Gerentes, int Operadores, int Activos, int Inactivos) ObtenerEstadisticas()
        {
            return _capaDatos.ObtenerEstadisticas();
        }

        public bool Registrar(Usuario u, out string mensaje)
        {
            return _capaDatos.Registrar(u, out mensaje);
        }

        public bool Actualizar(Usuario u, string dniOriginal, out string mensaje)
        {
            return _capaDatos.Actualizar(u, dniOriginal, out mensaje);
        }

        public bool CambiarEstado(string dni, int nuevoEstado)
        {
            return _capaDatos.CambiarEstado(dni, nuevoEstado);
        }
    }
}
