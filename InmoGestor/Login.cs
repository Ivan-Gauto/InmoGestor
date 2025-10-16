using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapaNegocio;
using CapaEntidades;

namespace InmoGestor
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            // Evita doble click mientras valida (opcional)
            BIngresar.Enabled = false;

            try
            {

                string dniText = TIngresoDNI.Text.Trim();
                string clave = TIngresoClave.Text.Trim();

                // Validaciones básicas
                bool camposFaltantes = string.IsNullOrWhiteSpace(dniText) || string.IsNullOrWhiteSpace(clave);

                if (camposFaltantes)
                {
                    MessageBox.Show("Debe completar todos los campos.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(dniText, out int dni))
                {
                    MessageBox.Show("El DNI debe ser numérico.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dniText.Length < 7 || dniText.Length > 8)
                {
                    MessageBox.Show("El DNI debe tener 7 u 8 dígitos.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Autenticación

                // Traer usuarios UNA sola vez
                List<Usuario> usuarios = new CN_Usuario().Listar();

                // Autenticación (usa las variables ya trimmeadas (sin espacios al inicio y final))
                Usuario oUsuario = usuarios.FirstOrDefault(
                    u => string.Equals(u.Dni?.Trim(), dniText, StringComparison.Ordinal)
                      && string.Equals(u.Clave, clave, StringComparison.Ordinal));

                if (oUsuario == null)
                {
                    MessageBox.Show("DNI o Clave incorrecta.", "Atención",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Si manejás Estado como bool en la entidad
                if (!oUsuario.Estado)
                {
                    MessageBox.Show("El usuario está inactivo.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // OK → abrir Inicio
                var form = new Inicio(oUsuario);

                form.FormClosing += frm_closing;
                form.Show();
                this.Hide();
            }

            catch (Exception ex)
            {
                MessageBox.Show("No se pudo validar el usuario.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BIngresar.Enabled = true;
            }
        }
        

        private void frm_closing(object sender, FormClosingEventArgs e)
        {   
            TIngresoDNI.Text = "";
            TIngresoClave.Text = "";
            this.Show();
        }
    }
}
