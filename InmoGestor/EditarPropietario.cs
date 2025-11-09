using CapaEntidades;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InmoGestor
{
    public partial class EditarPropietario : Form
    {
        private readonly PersonaRolCliente _propietarioEditar;

        public EditarPropietario(PersonaRolCliente propietario)
        {
            InitializeComponent();
            _propietarioEditar = propietario;
            this.Load += EditarPropietario_Load;
        }

        private void EditarPropietario_Load(object sender, EventArgs e)
        {
            if (_propietarioEditar != null && _propietarioEditar.oPersona != null)
            {
                TDni.Text = _propietarioEditar.Dni;
                TNombre.Text = _propietarioEditar.oPersona.Nombre;
                TApellido.Text = _propietarioEditar.oPersona.Apellido;
                TTelefono.Text = _propietarioEditar.oPersona.Telefono;
                TCorreo.Text = _propietarioEditar.oPersona.CorreoElectronico;
                TDireccion.Text = _propietarioEditar.oPersona.Direccion;
                dateTimePicker1.Value = _propietarioEditar.oPersona.FechaNacimiento;
            }
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
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

            if (dateTimePicker1.Value > DateTime.Now)
            {
                MessageBox.Show("La Fecha de nacimiento no puede ser una fecha futura.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker1.Focus();
                return;
            }

            string dniOriginal = _propietarioEditar.Dni;
            string dniNuevo = TDni.Text;

            _propietarioEditar.oPersona.Dni = dniNuevo;
            _propietarioEditar.oPersona.Nombre = TNombre.Text;
            _propietarioEditar.oPersona.Apellido = TApellido.Text;
            _propietarioEditar.oPersona.Telefono = TTelefono.Text;
            _propietarioEditar.oPersona.CorreoElectronico = TCorreo.Text;
            _propietarioEditar.oPersona.Direccion = TDireccion.Text;
            _propietarioEditar.oPersona.FechaNacimiento = dateTimePicker1.Value;

            string mensaje = string.Empty;
            CN_PersonaRolCliente negocio = new CN_PersonaRolCliente();

            bool exito = negocio.Actualizar(dniOriginal, _propietarioEditar, out mensaje);

            if (exito)
            {
                MessageBox.Show("Propietario actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al actualizar: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}