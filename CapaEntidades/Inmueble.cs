using System;
using System.Collections.Generic;

namespace CapaEntidades
{
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public string Direccion { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; } // Para la baja lógica (tu columna 'estado')
        public DateTime FechaCreacion { get; set; }

        public TipoInmueble oTipoInmueble { get; set; } // FK: id_tipo_inmueble
        public PersonaRolCliente oPropietario { get; set; } // FK: dni, rol_cliente_id

        public string RutaImagen { get; set; }
        public int Disponibilidad { get; set; } // 1=Disponible, 2=Ocupado, etc.
        public string Division { get; set; }

        // --- Propiedades calculadas (solo para mostrar en la vista) ---
        public string TipoInmuebleNombre => oTipoInmueble?.Nombre ?? "N/A";
        public string DisponibilidadNombre
        {
            get
            {
                switch (Disponibilidad)
                {
                    case 1: return "Disponible";
                    case 2: return "Ocupado";
                    case 3: return "Mantenimiento";
                    default: return "No especificado";
                }
            }
        }


        public string PropietarioNombreCompleto => $"{oPropietario?.oPersona?.Nombre} {oPropietario?.oPersona?.Apellido}".Trim();

        // Constructor
        public Inmueble()
        {
            oTipoInmueble = new TipoInmueble();
            oPropietario = new PersonaRolCliente();
            Estado = 1; // Por defecto, el inmueble está activo
            FechaCreacion = DateTime.Now;
        }
    }
}