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

        public CD_Usuario objcd_usuario = new CD_Usuario();

        public List<Usuario> Listar()
        {
            return objcd_usuario.Listar();
        }

        public bool Registrar(Usuario u, out string mensaje)
        {
            return objcd_usuario.Registrar(u, out mensaje);
        }

        public bool Actualizar(Usuario u, out string mensaje) =>
            objcd_usuario.Actualizar(u, out mensaje);


    }
}
