using CapaEntidades;
using CapaNegocio; // Asegúrate de tener la referencia a tu capa de negocio
using System;
using System.Windows.Forms;

namespace InmoGestor
{
    public partial class EditarInquilino : Form
    {
        // Variable privada para mantener los datos del inquilino que se está editando.
        private readonly PersonaRolCliente _inquilino;

        public EditarInquilino(PersonaRolCliente inquilino)
        {
            InitializeComponent();

            // Guardamos el objeto inquilino que recibimos. Si es nulo, lanzamos una excepción.
            _inquilino = inquilino ?? throw new ArgumentNullException(nameof(inquilino));

            // Asociamos los eventos a sus manejadores (handlers).
            this.Load += EditarInquilino_Load; // Para cargar los datos cuando el form se abre.
            BGuardar.Click += BGuardar_Click; // Para guardar los cambios.
        }

        private void EditarInquilino_Load(object sender, EventArgs e)
        {
            // --- Cargar datos del inquilino en los controles del formulario ---

            // El DNI es la clave primaria, no se debe poder editar.
            TDni.Text = _inquilino.Dni;
            TDni.Enabled = false;

            // Usamos el operador '?' para evitar errores si 'oPersona' es nulo.
            TNombre.Text = _inquilino.oPersona?.Nombre ?? "";
            TApellido.Text = _inquilino.oPersona?.Apellido ?? "";
            TCorreo.Text = _inquilino.oPersona?.CorreoElectronico ?? "";
            TTelefono.Text = _inquilino.oPersona?.Telefono ?? "";
            TDireccion.Text = _inquilino.oPersona?.Direccion ?? "";

            // Para el DateTimePicker, verificamos si la fecha tiene un valor antes de asignarla.
            if (_inquilino.oPersona?.FechaNacimiento.HasValue == true)
            {
                // Aseguramos que la fecha no esté fuera del rango soportado por el control.
                DateTime fechaNac = _inquilino.oPersona.FechaNacimiento.Value;
                if (fechaNac >= TNacimiento.MinDate && fechaNac <= TNacimiento.MaxDate)
                {
                    TNacimiento.Value = fechaNac;
                }
            }
        }

        private void BGuardar_Click(object sender, EventArgs e)
        {
            // --- Validaciones básicas antes de guardar ---
            if (string.IsNullOrWhiteSpace(TNombre.Text) || string.IsNullOrWhiteSpace(TApellido.Text))
            {
                MessageBox.Show("El Nombre y el Apellido son campos obligatorios.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Mapear los datos de los controles de vuelta al objeto _inquilino ---

            // Si por alguna razón oPersona fuera nulo, lo inicializamos.
            if (_inquilino.oPersona == null)
            {
                _inquilino.oPersona = new Persona { Dni = _inquilino.Dni };
            }

            // Asignamos los nuevos valores desde los TextBoxes, quitando espacios extra.
            _inquilino.oPersona.Nombre = TNombre.Text.Trim();
            _inquilino.oPersona.Apellido = TApellido.Text.Trim();
            _inquilino.oPersona.CorreoElectronico = TCorreo.Text.Trim();
            _inquilino.oPersona.Telefono = TTelefono.Text.Trim();
            _inquilino.oPersona.Direccion = TDireccion.Text.Trim();
            _inquilino.oPersona.FechaNacimiento = TNacimiento.Value;

            // --- Llamar a la capa de negocio para persistir los cambios ---
            var capaNegocio = new CN_PersonaRolCliente();
            string mensaje;

            bool resultado = capaNegocio.Actualizar(_inquilino, out mensaje);

            // --- Informar al usuario el resultado de la operación ---
            if (resultado)
            {
                MessageBox.Show("Inquilino actualizado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                // DialogResult.OK le informa al formulario padre que la operación fue exitosa.
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "Error al guardar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // El método para cerrar el formulario ya lo tenías, está perfecto.
        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Tenías este método duplicado, puedes borrarlo si quieres.
        private void BCerrarForm_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}