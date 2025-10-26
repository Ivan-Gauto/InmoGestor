using CapaEntidades;
using CapaNegocio;
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
    public partial class EditarInquilino : Form
    {
        private readonly PersonaRolCliente _inquilino;

        public EditarInquilino(PersonaRolCliente inquilino)
        {
            InitializeComponent();
            _inquilino = inquilino ?? throw new ArgumentNullException(nameof(inquilino));
            this.Load += EditarInquilino_Load;
            this.BGuardar.Click += new System.EventHandler(this.BGuardar_Click);
            this.BCerrarForm.Click += new System.EventHandler(this.BCerrarForm_Click);
        }

        private void EditarInquilino_Load(object sender, EventArgs e)
        {
            if (_inquilino != null && _inquilino.oPersona != null)
            {
                TDni.Text = _inquilino.Dni;
                TNombre.Text = _inquilino.oPersona.Nombre;
                TApellido.Text = _inquilino.oPersona.Apellido;
                TCorreo.Text = _inquilino.oPersona.CorreoElectronico;
                TTelefono.Text = _inquilino.oPersona.Telefono;
                TDireccion.Text = _inquilino.oPersona.Direccion;

                if (_inquilino.oPersona.FechaNacimiento >= TNacimiento.MinDate &&
                    _inquilino.oPersona.FechaNacimiento <= TNacimiento.MaxDate)
                {
                    TNacimiento.Value = _inquilino.oPersona.FechaNacimiento;
                }
                else
                {
                    TNacimiento.Value = DateTime.Now;
                }
            }
        }

        private void BGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TDni.Text))
            {
                MessageBox.Show("El campo DNI no puede estar vacío.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TDni.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TNombre.Text))
            {
                MessageBox.Show("El campo Nombre no puede estar vacío.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TApellido.Text))
            {
                MessageBox.Show("El campo Apellido no puede estar vacío.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TApellido.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(TCorreo.Text) && (!TCorreo.Text.Contains("@") || !TCorreo.Text.Contains(".")))
            {
                MessageBox.Show("El formato del Correo electrónico no es válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TCorreo.Focus();
                return;
            }

            if (TNacimiento.Value > DateTime.Now)
            {
                MessageBox.Show("La Fecha de nacimiento no puede ser una fecha futura.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }

            if (_inquilino.oPersona == null)
            {
                _inquilino.oPersona = new Persona { Dni = _inquilino.Dni };
            }

            string dniOriginal = _inquilino.Dni;
            string dniNuevo = TDni.Text.Trim();

            _inquilino.oPersona.Dni = dniNuevo;
            _inquilino.oPersona.Nombre = TNombre.Text.Trim();
            _inquilino.oPersona.Apellido = TApellido.Text.Trim();
            _inquilino.oPersona.CorreoElectronico = TCorreo.Text.Trim();
            _inquilino.oPersona.Telefono = TTelefono.Text.Trim();
            _inquilino.oPersona.Direccion = TDireccion.Text.Trim();
            _inquilino.oPersona.FechaNacimiento = TNacimiento.Value;

            var capaNegocio = new CN_PersonaRolCliente();
            string mensaje;

            bool resultado = capaNegocio.Actualizar(dniOriginal, _inquilino, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Inquilino actualizado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "Error al guardar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}