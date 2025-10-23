using CapaEntidades;
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
        // 1. Variable para guardar el propietario a editar
        private readonly PersonaRolCliente _propietarioEditar;

        // 2. Constructor modificado
        public EditarPropietario(PersonaRolCliente propietario)
        {
            InitializeComponent();
            _propietarioEditar = propietario; // Guardamos el propietario
            this.Load += EditarPropietario_Load; // Asociamos el evento Load
        }

        // 3. Evento Load para rellenar los campos
        private void EditarPropietario_Load(object sender, EventArgs e)
        {
            if (_propietarioEditar != null)
            {
                // Asumiendo que tus TextBoxes se llaman así (ej: TDni, txtNombre)
                TDni.Text = _propietarioEditar.Dni;
                TNombre.Text = _propietarioEditar.oPersona?.Nombre;
                TApellido.Text = _propietarioEditar.oPersona?.Apellido;
                TTelefono.Text = _propietarioEditar.oPersona?.Telefono;
                TCorreo.Text = _propietarioEditar.oPersona?.CorreoElectronico;
                TDireccion.Text = _propietarioEditar.oPersona?.Direccion;

                // (Si tienes un DateTimePicker para la fecha de nacimiento)
                // dtpFechaNacimiento.Value = _propietarioEditar.oPersona.FechaNacimiento;
            }
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- Pendiente ---
        // Aquí necesitarás la lógica para el botón "Guardar"
        // que llame a new CN_PersonaRolCliente().Actualizar(...)
    }
}