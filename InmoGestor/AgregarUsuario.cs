using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InmoGestor
{
    public partial class AgregarUsuario : Form
    {
        public AgregarUsuario()
        {
            InitializeComponent();
            BIngresar.Click += BIngresar_Click;   
        }

        private int MapRolId(string texto)
        {
            switch ((texto ?? "").Trim())
            {
                case "Administrador": return 1;
                case "Operador": return 2;
                case "Ayudante": return 3;
                default: return 2; // por defecto Operador
            }
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            // Validaciones mínimas
            string dni = TDni.Text.Trim();
            string clave = TClave.Text.Trim();
            string nombre = TNombre.Text.Trim();
            string apellido = TApellido.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(clave) ||
                string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Completá DNI, Clave, Nombre y Apellido.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!long.TryParse(dni, out _) || dni.Length < 7 || dni.Length > 8)
            {
                MessageBox.Show("El DNI debe ser numérico y de 7/8 dígitos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse fecha (tu textbox TNacimiento)
            DateTime? fnac = null;
            if (!string.IsNullOrWhiteSpace(TNacimiento.Text))
            {
                if (DateTime.TryParse(TNacimiento.Text.Trim(), out var f)) fnac = f;
                else
                {
                    MessageBox.Show("Fecha de nacimiento inválida (usa dd/mm/aaaa).", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int rolId = MapRolId(comboBox1.SelectedItem?.ToString());

            // Armar entidad
            var usuario = new CapaEntidades.Usuario
            {
                Dni = dni,
                Clave = clave,
                Estado = true,                   // activo
                FechaCreacion = DateTime.Now,
                RolUsuarioId = rolId,
                oPersona = new CapaEntidades.Persona
                {
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    CorreoElectronico = TCorreo.Text.Trim(),
                    Telefono = TTelefono.Text.Trim(),
                    Direccion = TDireccion.Text.Trim(),
                    Estado = 1,
                    FechaNacimiento = fnac
                }
            };

            // Guardar
            string mensaje;
            bool ok = new CapaNegocio.CN_Usuario().Registrar(usuario, out mensaje);

            if (!ok)
            {
                MessageBox.Show(mensaje, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Usuario creado correctamente.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }


        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AgregarUsuario_Load(object sender, EventArgs e)
        {

        }
    }
}
