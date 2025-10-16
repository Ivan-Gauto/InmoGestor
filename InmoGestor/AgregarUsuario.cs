using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace InmoGestor
{
    public partial class AgregarUsuario : Form
    {
        public AgregarUsuario()
        {
            InitializeComponent();
            BIngresar.Click += BIngresar_Click;   
        }

        private int MapRolId(string texto)
        {
            switch ((texto ?? "").Trim())
            {
                case "Administrador": return 1;
                case "Operador": return 2;
                case "Ayudante": return 3;
                default: return 2; // por defecto Operador
            }
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            // Validaciones mínimas
            string dni = TDni.Text.Trim();
            string clave = TClave.Text.Trim();
            string nombre = TNombre.Text.Trim();
            string apellido = TApellido.Text.Trim();

            if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(clave) ||
                string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Completá DNI, Clave, Nombre y Apellido.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!long.TryParse(dni, out _) || dni.Length < 7 || dni.Length > 8)
            {
                MessageBox.Show("El DNI debe ser numérico y de 7/8 dígitos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // Clave: 8 o más dígitos
            if (!Regex.IsMatch(clave, @"^\d{8,}$"))
            {
                MessageBox.Show("La contraseña debe tener al menos 8 dígitos numéricos.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TClave.Focus();
                return;
            }

            //validar edad mínima
            var edad = DateTime.Today.Year - fnacValue.Year;
            if (fnacValue.Date > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 17)
            {
                MessageBox.Show("El usuario debe tener al menos 17 años.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TNacimiento.Focus();
                return;
            }


            int rolId = MapRolId(comboBox1.SelectedItem?.ToString());

            // Armar entidad
            var usuario = new CapaEntidades.Usuario
            {
                Dni = dni,
                Clave = clave,
                Estado = true,                   // activo
                FechaCreacion = DateTime.Now,
                RolUsuarioId = rolId,
                oPersona = new CapaEntidades.Persona
                {
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    CorreoElectronico = TCorreo.Text.Trim(),
                    Telefono = TTelefono.Text.Trim(),
                    Direccion = TDireccion.Text.Trim(),
                    Estado = 1,
                    FechaNacimiento = fnacValue
                }
            };

            // Guardar
            string mensaje;
            bool ok = new CapaNegocio.CN_Usuario().Registrar(usuario, out mensaje);

            if (!ok)
            {
                MessageBox.Show(mensaje, "No se pudo guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Usuario creado correctamente.", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }


        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AgregarUsuario_Load(object sender, EventArgs e)
        {

        }
    }
}
