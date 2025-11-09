// /CapaNegocio/CN_Cuota.cs
using System.Collections.Generic;
using CapaEntidades;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Cuota
    {
        CD_Cuota cdCuota = new CD_Cuota();
        public List<Cuota> ListarPorContrato(int contratoId)
        {
            return cdCuota.ListarPorContrato(contratoId);
        }
    }
}
