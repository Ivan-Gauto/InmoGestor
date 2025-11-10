using CapaEntidades;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InmoGestor
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // Ajustes de UX
            this.AcceptButton = BIngresar;  // Enter = click en Ingresar
            TIngresoDNI.Focus();
            var cnContrato = new CN_Contrato();
            cnContrato.ActualizarContratosFinalizados();
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            BIngresar.Enabled = false;

            try
            {
                string dniText = TIngresoDNI.Text.Trim();
                string clave = TIngresoClave.Text.Trim();

                if (string.IsNullOrWhiteSpace(dniText))
                {
                    MessageBox.Show("Debe completar su dni", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TIngresoDNI.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(clave))
                {
                    MessageBox.Show("Debe completar su contraseña", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TIngresoClave.Focus();
                    return;
                }

                // (Tus validaciones de Regex están bien)
                if (!long.TryParse(dniText, out _) || dniText.Length != 8)
                {
                    MessageBox.Show("El DNI debe ser numérico y tener 8 dígitos.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TIngresoDNI.Focus();
                    return;
                }
                if (!Regex.IsMatch(clave, @"^\d{8,}$"))
                {
                    MessageBox.Show("La contraseña debe tener al menos 8 dígitos numéricos.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TIngresoClave.Focus();
                    return;
                }

                // --- ¡AQUÍ ESTÁ EL CAMBIO! ---
                // 1. Llamamos al nuevo método eficiente
                CN_Usuario cnUsuario = new CN_Usuario();
                Usuario oUsuario = cnUsuario.ValidarUsuario(dniText, clave);

                // 2. Verificamos el resultado
                if (oUsuario == null)
                {
                    MessageBox.Show("DNI o Clave incorrecta.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (oUsuario.Estado == 0)
                {
                    MessageBox.Show("Su cuenta de usuario se encuentra desactivada. Contacte a un administrador.", "Usuario inactivo",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // --- ¡ÉXITO! ---
                // oUsuario AHORA SÍ CONTIENE EL 'UsuarioId' (ej. 1, 2, 3...)
                SesionUsuario.IniciarSesion(oUsuario);

                var form = new Inicio(oUsuario);
                form.FormClosing += frm_closing;
                form.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo validar el usuario.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // --- REGISTRO DE ERROR ---
                // Ahora esto funcionará, porque si el login falla (ej. error de BD),
                // el usuarioId será null, no 0.
                new CN_Reportes().RegistrarError(null, "Login", "BIngresar_Click", ex);
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