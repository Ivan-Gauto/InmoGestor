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
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            BIngresar.Enabled = false;

            try
            {
                string dniText = TIngresoDNI.Text.Trim();
                string clave = TIngresoClave.Text.Trim();

                // --- Tus validaciones (están bien) ---
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

                // (Considera ajustar esta validación si tu DNI puede tener otro formato)
                if (!long.TryParse(dniText, out _) || dniText.Length != 8)
                {
                    MessageBox.Show("El DNI debe ser numérico y tener 8 dígitos.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TIngresoDNI.Focus();
                    return;
                }

                // (Considera ajustar esta validación si tu clave tiene otro formato)
                if (!Regex.IsMatch(clave, @"^\d{8,}$"))
                {
                    MessageBox.Show("La contraseña debe tener al menos 8 dígitos numéricos.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TIngresoClave.Focus();
                    return;
                }

                // --- Validación de Credenciales ---
                // Mejora Sugerida: Es más eficiente crear un método
                // CN_Usuario.ValidarCredenciales(dni, clave) que haga la consulta
                // filtrando en la base de datos, en lugar de traer todos los usuarios.
                List<Usuario> usuarios = new CN_Usuario().Listar(RolUsuarioFiltro.Todos, EstadoFiltro.Todos);

                Usuario oUsuario = usuarios.FirstOrDefault(
                    u => string.Equals(u.Dni?.Trim(), dniText, StringComparison.Ordinal)
                      && string.Equals(u.Clave, clave, StringComparison.Ordinal)); // CUIDADO: Comparar claves en texto plano es inseguro. Deberías usar hashes.

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

                // --- ¡ÉXITO! Guardar Sesión y Abrir Formulario Principal ---
                SesionUsuario.IniciarSesion(oUsuario); // <-- LÍNEA AGREGADA

                var form = new Inicio(oUsuario); // Ya le pasabas el usuario a Inicio, ¡perfecto!
                form.FormClosing += frm_closing;
                form.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo validar el usuario.\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Aquí deberías registrar el error en tu tabla log_error
                // CN_Log.RegistrarError(0, "Login", "BIngresar_Click", ex.Message); // (0 si no sabes el usuario_id)
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