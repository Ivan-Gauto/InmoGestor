// CapaNegocio/CN_Recibo.cs
using CapaDatos;
using CapaEntidades;
using System;

namespace CapaNegocio
{
    public class CN_Recibo
    {
        private readonly CD_Recibo _cd = new CD_Recibo();

        public long CrearParaPago(long pagoId)
        {
            if (_cd.ExisteParaPago(pagoId))
                return 0; // ya existe, idempotente

            return _cd.CrearParaPago(pagoId);
        }

        public Recibo ObtenerPorPago(long pagoId) => _cd.ObtenerPorPago(pagoId);
    }
}
