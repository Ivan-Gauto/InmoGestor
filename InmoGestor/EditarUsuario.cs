using System;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace InmoGestor
{
    public partial class EditarUsuario : Form
    {
        private Usuario _modelo;

       
        public EditarUsuario(Usuario usuario)
        {
            InitializeComponent();
            _modelo = usuario ?? throw new ArgumentNullException(nameof(usuario));
            BGuardar.Click += BGuardar_Click; // WIRE del botón
        }

        private int MapRolId(string texto)
        {
            var t = (texto ?? "").Trim().ToUpperInvariant();
            if (t == "ADMINISTRADOR") return 1;
            if (t == "OPERADOR") return 2;
            if (t == "AYUDANTE") return 3;
            // por si tu combo usa “bonitos”
            if (t == "ADMINISTRADOR") return 1;
            return 2;
        }

        private void EditarUsuario_Load(object sender, EventArgs e)
        {
            // Si el combo no está cargado desde el diseñador, cargalo:
            if (comboRol.Items.Count == 0)
            {
                comboRol.Items.AddRange(new object[] { "Administrador", "Operador", "Ayudante" });
            }

            TDni.Text = _modelo.Dni;
            TDni.Enabled = false;  // PK, no editable
            TNombre.Text = _modelo.oPersona?.Nombre ?? "";
            TApellido.Text = _modelo.oPersona?.Apellido ?? "";
            TCorreo.Text = _modelo.oPersona?.CorreoElectronico ?? "";
            TTelefono.Text = _modelo.oPersona?.Telefono ?? "";
            TDireccion.Text = _modelo.oPersona?.Direccion ?? "";
            TClave.Text = _modelo.Clave ?? "";
            TNacimiento.Text = _modelo.oPersona.FechaNacimiento
                .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            checkEstado.Checked = _modelo.Estado;

            // Seleccionar rol en combo por nombre (tolerante a may/min)
            var rolNombre = _modelo.oRolUsuario?.Nombre ?? (
                _modelo.RolUsuarioId == 1 ? "Administrador" :
                _modelo.RolUsuarioId == 2 ? "Operador" : "Ayudante");

            // Buscar ítem coincidente ignorando mayúsculas
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
            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(TNombre.Text) ||
                string.IsNullOrWhiteSpace(TApellido.Text) ||
                string.IsNullOrWhiteSpace(TClave.Text))
            {
                MessageBox.Show("Completá Nombre, Apellido y Clave.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // === Fecha de nacimiento OBLIGATORIA, formato dd/MM/yyyy ===
            if (string.IsNullOrWhiteSpace(TNacimiento.Text))
            {
                MessageBox.Show("La fecha de nacimiento es obligatoria.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            // Aceptar varios formatos y luego normalizar a dd/MM/yyyy
            string input = TNacimiento.Text.Trim();
            string[] formatos = { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy-M-d", "yyyy/M/d", "dd-MM-yyyy", "d-M-yyyy" };

            if (!DateTime.TryParseExact(input, formatos, new CultureInfo("es-AR"),
                                        DateTimeStyles.None, out var fnacValue))
            {
                MessageBox.Show("Fecha de nacimiento inválida. Usá el formato dd/mm/aaaa.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            // Normalizar el texto del TextBox a dd/MM/yyyy (mas prolijo)
            TNacimiento.Text = fnacValue.ToString("dd/MM/yyyy");

            if (fnacValue.Date > DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            // (Opcional) validar edad mínima
            var edad = DateTime.Today.Year - fnacValue.Year;
            if (fnacValue.Date > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 17)
            {
                MessageBox.Show("El usuario debe tener al menos 17 años.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            // Clave: 8 o más dígitos
            var clave = TClave.Text.Trim();
            if (!Regex.IsMatch(clave, @"^\d{8,}$"))
            {
                MessageBox.Show("La contraseña debe tener al menos 8 dígitos numéricos.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TClave.Focus();
                return;
            }

            // Mapear a modelo
            _modelo.Clave = TClave.Text.Trim();
            _modelo.Estado = checkEstado.Checked;
            _modelo.RolUsuarioId = MapRolId(comboRol.SelectedItem?.ToString());

            if (_modelo.oPersona == null) _modelo.oPersona = new Persona { Dni = _modelo.Dni };
            _modelo.oPersona.Nombre = TNombre.Text.Trim();
            _modelo.oPersona.Apellido = TApellido.Text.Trim();
            _modelo.oPersona.CorreoElectronico = TCorreo.Text.Trim();
            _modelo.oPersona.Telefono = TTelefono.Text.Trim();
            _modelo.oPersona.Direccion = TDireccion.Text.Trim();
            _modelo.oPersona.Estado = 1; // en BD persona.estado = INT
            _modelo.oPersona.FechaNacimiento = fnacValue.Date;   // <-- NO nullable

            // Persistir
            string msg;
            bool ok = new CN_Usuario().Actualizar(_modelo, out msg);
            if (!ok)
            {
                MessageBox.Show(msg, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Usuario actualizado.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close(); // al cerrar, Usuarios refresca la grilla (CargarUsuarios)
        }
        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
