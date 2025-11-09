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
        // Se elimina _usuarioLogueado ya que la información se obtiene de SesionUsuario
        private string _dniOriginal;

        // Se elimina el parámetro usuarioLogueado del constructor
        public EditarUsuario(Usuario usuarioAEditar)
        {
            InitializeComponent();
            _modelo = usuarioAEditar ?? throw new ArgumentNullException(nameof(usuarioAEditar));
            _dniOriginal = _modelo.Dni; // Guardar DNI original al inicio
            this.Load += EditarUsuario_Load; // Asociar Load explícitamente si no está en el diseñador
            BGuardar.Click += BGuardar_Click;
            BCerrarForm.Click += BCerrarForm_Click; // Asegurarse que el botón de cerrar esté conectado
        }

        private int MapRolId(string texto)
        {
            var t = (texto ?? "").Trim().ToUpperInvariant();
            if (t == "ADMINISTRADOR") return 1;
            if (t == "GERENTE") return 2;
            if (t == "OPERADOR") return 3;
            return 3; // Devuelve Operador por defecto si no coincide
        }

        private void EditarUsuario_Load(object sender, EventArgs e)
        {
            if (comboRol.Items.Count == 0)
            {
                comboRol.Items.AddRange(new object[] { "Administrador", "Gerente", "Operador" });
            }

            TDni.Text = _modelo.Dni;
            TNombre.Text = _modelo.oPersona?.Nombre ?? "";
            TApellido.Text = _modelo.oPersona?.Apellido ?? "";
            TCorreo.Text = _modelo.oPersona?.CorreoElectronico ?? "";
            TTelefono.Text = _modelo.oPersona?.Telefono ?? "";
            TDireccion.Text = _modelo.oPersona?.Direccion ?? "";
            TClave.Text = _modelo.Clave ?? ""; // Considera no mostrar la clave o usar máscara

            if (_modelo.oPersona != null && _modelo.oPersona.FechaNacimiento >= TNacimiento.MinDate && _modelo.oPersona.FechaNacimiento <= TNacimiento.MaxDate)
            {
                TNacimiento.Value = _modelo.oPersona.FechaNacimiento;
            }
            else if (_modelo.oPersona != null) // Si la fecha está fuera de rango, poner una válida
            {
                TNacimiento.Value = DateTime.Today.AddYears(-18); // Por ejemplo, 18 años atrás
            }

            string adminDniActual = SesionUsuario.ObtenerDniUsuarioActual();

            // Deshabilitar cambio de rol si se está editando a sí mismo
            if (_modelo.Dni == adminDniActual)
            {
                comboRol.Enabled = false;
            }

            var rolNombre = _modelo.oRolUsuario?.Nombre ?? (
                _modelo.RolUsuarioId == 1 ? "Administrador" :
                _modelo.RolUsuarioId == 2 ? "Gerente" : "Operador");

            // Seleccionar el rol actual en el ComboBox
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
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(TDni.Text))
            {
                MessageBox.Show("El campo DNI no puede estar vacío.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TDni.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(TNombre.Text) ||
               string.IsNullOrWhiteSpace(TApellido.Text) ||
               string.IsNullOrWhiteSpace(TClave.Text))
            {
                MessageBox.Show("Completá DNI, Nombre, Apellido y Clave.",
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
            if (!Regex.IsMatch(clave, @"^\d{8,}$")) // Ajusta Regex si la clave no es solo numérica
            {
                MessageBox.Show("La contraseña debe tener al menos 8 caracteres.", // Mensaje genérico si no es solo numérica
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TClave.Focus();
                return;
            }
            if (!string.IsNullOrWhiteSpace(TCorreo.Text) && (!TCorreo.Text.Contains("@") || !TCorreo.Text.Contains(".")))
            {
                MessageBox.Show("El formato del Correo electrónico no es válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TCorreo.Focus();
                return;
            }

            // Actualizar el objeto _modelo con los datos del formulario
            _modelo.Dni = TDni.Text.Trim();
            _modelo.Clave = TClave.Text.Trim(); // Considera hacer HASH aquí si no lo haces en capa de negocio/datos
            _modelo.RolUsuarioId = MapRolId(comboRol.SelectedItem?.ToString());

            if (_modelo.oPersona == null) _modelo.oPersona = new Persona();
            _modelo.oPersona.Dni = TDni.Text.Trim(); // Asegurarse que oPersona también tenga el DNI actualizado
            _modelo.oPersona.Nombre = TNombre.Text.Trim();
            _modelo.oPersona.Apellido = TApellido.Text.Trim();
            _modelo.oPersona.CorreoElectronico = TCorreo.Text.Trim();
            _modelo.oPersona.Telefono = TTelefono.Text.Trim();
            _modelo.oPersona.Direccion = TDireccion.Text.Trim();
            _modelo.oPersona.FechaNacimiento = TNacimiento.Value;

            // Obtener DNI del administrador que realiza la acción
            string adminDniActual = SesionUsuario.ObtenerDniUsuarioActual();
            if (string.IsNullOrEmpty(adminDniActual))
            {
                MessageBox.Show("Error: No se pudo identificar al administrador actual para registrar la auditoría.", "Error de Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Detener si no hay admin logueado
            }

            string msg;
            bool ok = new CN_Usuario().Actualizar(_modelo, _dniOriginal, adminDniActual, out msg);

            if (!ok)
            {
                MessageBox.Show(msg, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Futuro: Registrar error en log_error
                // CN_Log.RegistrarError(adminDniActual, "EditarUsuario", "BGuardar_Click", msg);
                return;
            }

            MessageBox.Show("Usuario actualizado.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK; // Indicar éxito al formulario padre
            Close();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Indicar cancelación
            Close();
        }
    }
}