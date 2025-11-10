using CapaDatos;
using CapaEntidades;
using System.Collections.Generic;
using System.Text; // Para las validaciones

namespace CapaNegocio
{
    public class CN_Inmueble
    {
        private CD_Inmueble objDatos = new CD_Inmueble();

        public List<Inmueble> Listar(bool incluirInactivos = false)
        {
            return objDatos.Listar(incluirInactivos);
        }

        public bool ReactivarLogico(int idInmueble)
        {
            return objDatos.ReactivarLogico(idInmueble);
        }

        public List<Inmueble> ListarPorPropietario(string dni, int rolId)
        {
            return objDatos.ListarPorPropietario(dni, rolId);
        }

        public List<TipoInmueble> ListarTiposInmueble()
        {
            // objDatos es tu instancia de CD_Inmueble
            return objDatos.ListarTiposInmueble();
        }

        // --- Registrar (Nuevo) ---
        public bool Registrar(Inmueble obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            StringBuilder sb = new StringBuilder();

            // --- Validaciones de Negocio ---
            if (string.IsNullOrWhiteSpace(obj.Direccion))
            {
                sb.AppendLine("La dirección no puede estar vacía.");
            }
            if (obj.oTipoInmueble.IdTipoInmueble == 0)
            {
                sb.AppendLine("Debe seleccionar un tipo de inmueble.");
            }
            if (string.IsNullOrWhiteSpace(obj.oPropietario.Dni))
            {
                sb.AppendLine("Debe seleccionar un propietario.");
            }
            if (obj.Disponibilidad == 0)
            {
                sb.AppendLine("Debe seleccionar una disponibilidad (Disponible, Ocupado, etc.).");
            }

            if (sb.Length > 0)
            {
                Mensaje = sb.ToString();
                return false;
            }

            // Si pasa las validaciones, llama a la capa de datos
            return objDatos.Registrar(obj, out Mensaje);
        }

        public bool DesactivarLogico(int idInmueble)
        {
            return objDatos.DesactivarLogico(idInmueble);
        }

        // --- Actualizar (Editar) ---
        public bool Actualizar(Inmueble obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            StringBuilder sb = new StringBuilder();

            // Validaciones similares a Registrar
            if (obj.IdInmueble <= 0)
            {
                sb.AppendLine("Id de inmueble inválido.");
            }
            if (string.IsNullOrWhiteSpace(obj.Direccion))
            {
                sb.AppendLine("La dirección no puede estar vacía.");
            }
            if (obj.oTipoInmueble == null || obj.oTipoInmueble.IdTipoInmueble == 0)
            {
                sb.AppendLine("Debe seleccionar un tipo de inmueble.");
            }
            if (obj.oPropietario == null || string.IsNullOrWhiteSpace(obj.oPropietario.Dni))
            {
                sb.AppendLine("Debe seleccionar un propietario.");
            }

            if (sb.Length > 0)
            {
                Mensaje = sb.ToString();
                return false;
            }

            // Llamar a la capa de datos para actualizar
            return objDatos.Actualizar(obj, out Mensaje);
        }
    }
}
