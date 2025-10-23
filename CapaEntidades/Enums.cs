using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public enum TipoRolCliente
    {
        Propietario = 1,
        Inquilino = 2
    }
    public enum EstadoFiltro
    {
        Todos,
        Activos,
        Inactivos,
        Morosos
    }

    public enum RolUsuarioFiltro
    {
        Todos,
        Administradores,
        Gerentes,
        Operadores
    }
}
