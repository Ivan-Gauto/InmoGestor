using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class AgregarUsuario : Form
    {
        public AgregarUsuario()
        {
            InitializeComponent();
            BIngresar.Click += BIngresar_Click;
            BCerrarForm.Click += BCerrarForm_Click; // Asegurarse que el botón cerrar esté conectado
            this.Load += AgregarUsuario_Load; // Conectar evento Load
        }

        private int MapRolId(string texto)
        {
            switch ((texto ?? "").Trim().ToUpperInvariant()) // Usar ToUpperInvariant para comparación sin distinción cultural
            {
                case "ADMINISTRADOR": return 1;
                case "GERENTE": return 2;
                case "OPERADOR": return 3;
                default: return 3; // Devolver Operador por defecto
            }
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            string dni = TDni.Text.Trim();
            string clave = TClave.Text.Trim();
            string nombre = TNombre.Text.Trim();
            string apellido = TApellido.Text.Trim();
            string correo = TCorreo.Text.Trim();
            string telefono = TTelefono.Text.Trim();
            string direccion = TDireccion.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(clave) ||
                string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Completá DNI, Clave, Nombre y Apellido.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!long.TryParse(dni, out _) || dni.Length < 7 || dni.Length > 8)
            {
                MessageBox.Show("El DNI debe ser numérico y de 7 u 8 dígitos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TDni.Focus();
                return;
            }

            DateTime fnacValue = TNacimiento.Value;

            if (fnacValue.Date > DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(correo) && (!correo.Contains("@") || !correo.Contains(".")))
            {
                MessageBox.Show("El formato del Correo electrónico no es válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TCorreo.Focus();
                return;
            }


            if (!Regex.IsMatch(clave, @"^.{8,}$")) // Permitir cualquier caracter, mínimo 8
            {
                MessageBox.Show("La contraseña debe tener al menos 8 caracteres.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TClave.Focus();
                return;
            }

            var edad = DateTime.Today.Year - fnacValue.Year;
            if (fnacValue.Date > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 17)
            {
                MessageBox.Show("El usuario debe tener al menos 17 años.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar un Rol para el usuario.", "Atención",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }

            int rolId = MapRolId(comboBox1.SelectedItem?.ToString());

            var usuario = new Usuario
            {
                Dni = dni,
                Clave = clave, // Considera hacer HASH de la clave antes de guardar
                Estado = 1, // Usuario nuevo siempre activo por defecto
                FechaCreacion = DateTime.Now,
                RolUsuarioId = rolId,
                oPersona = new Persona
                {
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    CorreoElectronico = correo,
                    Telefono = telefono,
                    Direccion = direccion,
                    Estado = 1, // Persona nueva siempre activa por defecto
                    FechaNacimiento = fnacValue.Date
                }
            };

            string adminDniActual = SesionUsuario.ObtenerDniUsuarioActual();
            if (string.IsNullOrEmpty(adminDniActual))
            {
                MessageBox.Show("Error: No se pudo identificar al administrador actual para registrar la auditoría.", "Error de Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string mensaje;
            bool ok = new CN_Usuario().Registrar(usuario, adminDniActual, out mensaje);

            if (!ok)
            {
                MessageBox.Show(mensaje, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Futuro: Registrar error en log_error
                // CN_Log.RegistrarError(adminDniActual, "AgregarUsuario", "BIngresar_Click", mensaje);
                return;
            }

            MessageBox.Show("Usuario creado correctamente.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK; // Indicar éxito al form padre
            this.Close();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Indicar cancelación
            this.Close();
        }

        private void AgregarUsuario_Load(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.AddRange(new object[] { "Administrador", "Gerente", "Operador" });
                comboBox1.SelectedIndex = 2; // Seleccionar Operador por defecto
            }
        }
    }
}