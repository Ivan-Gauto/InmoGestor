using System;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;
using System.Globalization;
using System.Text.RegularExpressions;

namespace InmoGestor
{
    public partial class EditarUsuario : Form
    {
        private Usuario _modelo;
        private Usuario _usuarioLogueado;
        private string _dniOriginal;

        public EditarUsuario(Usuario usuarioAEditar, Usuario usuarioLogueado)
        {
            InitializeComponent();
            _modelo = usuarioAEditar ?? throw new ArgumentNullException(nameof(usuarioAEditar));
            _usuarioLogueado = usuarioLogueado;
            BGuardar.Click += BGuardar_Click;
        }

        private int MapRolId(string texto)
        {
            var t = (texto ?? "").Trim().ToUpperInvariant();
            if (t == "ADMINISTRADOR") return 1;
            if (t == "GERENTE") return 2;
            if (t == "OPERADOR") return 3;
            return 2;
        }

        private void EditarUsuario_Load(object sender, EventArgs e)
        {
            if (comboRol.Items.Count == 0)
            {
                comboRol.Items.AddRange(new object[] { "Administrador", "Gerente", "Operador" });
            }

            _dniOriginal = _modelo.Dni;
            TDni.Text = _modelo.Dni;
            TNombre.Text = _modelo.oPersona?.Nombre ?? "";
            TApellido.Text = _modelo.oPersona?.Apellido ?? "";
            TCorreo.Text = _modelo.oPersona?.CorreoElectronico ?? "";
            TTelefono.Text = _modelo.oPersona?.Telefono ?? "";
            TDireccion.Text = _modelo.oPersona?.Direccion ?? "";
            TClave.Text = _modelo.Clave ?? "";

            if (_modelo.oPersona != null)
            {
                TNacimiento.Value = _modelo.oPersona.FechaNacimiento;
            }

            if (_modelo.Dni == _usuarioLogueado.Dni)
            {
                comboRol.Enabled = false;
            }

            var rolNombre = _modelo.oRolUsuario?.Nombre ?? (
                _modelo.RolUsuarioId == 1 ? "Administrador" :
                _modelo.RolUsuarioId == 2 ? "Gerente" : "Operador");

            for (int i = 0; i < comboRol.Items.Count; i++)
            {
                if (string.Equals(comboRol.Items[i].ToString(), rolNombre, StringComparison.OrdinalIgnoreCase))
                {
                    comboRol.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TNombre.Text) ||
                string.IsNullOrWhiteSpace(TApellido.Text) ||
                string.IsNullOrWhiteSpace(TClave.Text))
            {
                MessageBox.Show("Completá Nombre, Apellido y Clave.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            var edad = DateTime.Today.Year - fnacValue.Year;
            if (fnacValue.Date > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 17)
            {
                MessageBox.Show("El usuario debe tener al menos 17 años.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            var clave = TClave.Text.Trim();
            if (!Regex.IsMatch(clave, @"^\d{8,}$"))
            {
                MessageBox.Show("La contraseña debe tener al menos 8 dígitos numéricos.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TClave.Focus();
                return;
            }

            _modelo.Dni = TDni.Text.Trim();
            _modelo.Clave = TClave.Text.Trim();
            _modelo.RolUsuarioId = MapRolId(comboRol.SelectedItem?.ToString());

            if (_modelo.oPersona == null) _modelo.oPersona = new Persona();
            _modelo.oPersona.Dni = TDni.Text.Trim();
            _modelo.oPersona.Nombre = TNombre.Text.Trim();
            _modelo.oPersona.Apellido = TApellido.Text.Trim();
            _modelo.oPersona.CorreoElectronico = TCorreo.Text.Trim();
            _modelo.oPersona.Telefono = TTelefono.Text.Trim();
            _modelo.oPersona.Direccion = TDireccion.Text.Trim();
            _modelo.oPersona.FechaNacimiento = TNacimiento.Value;

            string msg;
            bool ok = new CN_Usuario().Actualizar(_modelo, _dniOriginal, out msg);

            if (!ok)
            {
                MessageBox.Show(msg, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Usuario actualizado.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}