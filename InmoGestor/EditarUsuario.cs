using System;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

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
            TNacimiento.Text = _modelo.oPersona?.FechaNacimiento?.ToString("yyyy-MM-dd") ?? "";
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

            // Fecha opcional
            DateTime? fnac = null;
            if (!string.IsNullOrWhiteSpace(TNacimiento.Text))
            {
                if (DateTime.TryParse(TNacimiento.Text.Trim(), out var f)) fnac = f;
                else
                {
                    MessageBox.Show("Fecha de nacimiento inválida.",
                        "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
            _modelo.oPersona.FechaNacimiento = fnac;
            _modelo.oPersona.Estado = 1; // en BD persona.estado = INT

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
