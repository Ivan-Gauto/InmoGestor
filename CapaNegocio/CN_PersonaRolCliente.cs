using CapaDatos;
using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_PersonaRolCliente
    {
        private readonly CD_PersonaRolCliente _data = new CD_PersonaRolCliente();
        public List<PersonaRolCliente> ListarInquilinos(int rolInquilinoId = 2) => _data.ListarInquilinos(rolInquilinoId);
    }

}
