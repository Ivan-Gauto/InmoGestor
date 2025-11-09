using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class AgregarPropietario : Form
    {
        public AgregarPropietario()
        {
            InitializeComponent();
            // Asigna el evento Click al botón BIngresar
            this.BIngresar.Click += new System.EventHandler(this.BIngresar_Click);
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Evento para el botón BIngresar
        private void BIngresar_Click(object sender, EventArgs e)
        {
            // Asegúrate de que TApellido exista en tu diseño
            if (string.IsNullOrWhiteSpace(TDni.Text) ||
                string.IsNullOrWhiteSpace(TNombre.Text) ||
                string.IsNullOrWhiteSpace(TApellido.Text))
            {
                MessageBox.Show("Debe completar los campos DNI, Nombre y Apellido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Persona oPersona = new Persona
            {
                Dni = TDni.Text,
                Nombre = TNombre.Text,
                Apellido = TApellido.Text, // <-- Verifica este nombre
                Telefono = TTelefono.Text,
                CorreoElectronico = TCorreo.Text,
                Direccion = TDireccion.Text,
                FechaNacimiento = dateTimePicker1.Value,
                Estado = 1
            };

            PersonaRolCliente nuevoPropietario = new PersonaRolCliente
            {
                Dni = TDni.Text,
                oPersona = oPersona,
                oRolCliente = new RolCliente
                {
                    RolClienteId = (int)TipoRolCliente.Propietario
                },
                Estado = 1
            };

            string mensaje = string.Empty;
            CN_PersonaRolCliente negocio = new CN_PersonaRolCliente();
            bool exito = negocio.Registrar(nuevoPropietario, out mensaje);

            if (exito)
            {
                MessageBox.Show("Propietario registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al registrar: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}