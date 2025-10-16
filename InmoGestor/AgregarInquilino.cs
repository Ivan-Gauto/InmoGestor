using System;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class AgregarInquilino : Form
    {
        public AgregarInquilino()
        {
            InitializeComponent();
            BIngresar.Click += BIngresar_Click;
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            string dni = TDni.Text.Trim();
            string nombre = TNombre.Text.Trim();
            string apellido = TApellido.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Completá DNI, Nombre y Apellido.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!long.TryParse(dni, out _) || dni.Length < 7 || dni.Length > 8)
            {
                MessageBox.Show("El DNI debe ser numérico y de 7 u 8 dígitos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si el DateTimePicker tiene check, usamos su valor;
            // si no, usamos una fecha por defecto (por ejemplo, hoy)
            DateTime fechaNac = dateTimePicker1.Checked
                ? dateTimePicker1.Value
                : DateTime.Today;

            var inquilino = new PersonaRolCliente
            {
                Dni = dni,
                oRolCliente = new RolCliente { RolClienteId = 2 },
                oPersona = new Persona
                {
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    CorreoElectronico = TCorreo.Text.Trim(),
                    Telefono = TTelefono.Text.Trim(),
                    Direccion = TDireccion.Text.Trim(),
                    Estado = 1,
                    FechaNacimiento = fechaNac // ✅ siempre un DateTime
                }
            };


            string mensaje;
            bool ok = new CN_PersonaRolCliente().Registrar(inquilino, out mensaje);

            if (!ok)
            {
                MessageBox.Show(mensaje, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Inquilino creado correctamente.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}