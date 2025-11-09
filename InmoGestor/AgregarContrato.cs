using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class AgregarContrato : Form
    {
        public AgregarContrato()
        {
            InitializeComponent();

            // Eventos UI
            this.Load += AgregarContrato_Load;
            this.BIngresar.Click += BIngresar_Click;
            this.BCerrarForm.Click += BCerrarForm_Click;

            this.dateTimePicker_Inicio.ValueChanged += DateInputs_Changed;
            this.TNacimiento.TextChanged += DateInputs_Changed; // Cantidad de cuotas
        }

        private void AgregarContrato_Load(object sender, EventArgs e)
        {
            try
            {
                // Defaults
                dateTimePicker_Inicio.Value = DateTime.Today;
                numericUpDown_TasaMensual.DecimalPlaces = 2;
                numericUpDown_TasaMensual.Minimum = 0;
                numericUpDown_TasaMensual.Maximum = 100;
                numericUpDown_TasaMensual.Value = 5;   // ejemplo
                TNacimiento.Text = "12";               // cuotas por defecto
                TNombre.Text = "0";                    // precio por cuota
                CalcularFechaFin();

                // Cargar combos
                CargarCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar la pantalla: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarCombos()
        {
            // Inmuebles disponibles
            var cnInmueble = new CN_Inmueble();
            List<Inmueble> inmuebles = cnInmueble.ListarDisponibles();

            // IMPORTANTE: estos nombres deben existir en los objetos de la lista
            // IdInmueble y Direccion (usa alias en tu SELECT de la capa de datos)
            comboBox1.ValueMember = nameof(Inmueble.IdInmueble);
            comboBox1.DisplayMember = nameof(Inmueble.Direccion);
            comboBox1.DataSource = inmuebles;

            // Inquilinos activos
            var cnPRC = new CN_PersonaRolCliente();
            // Usamos tu firma existente:
            // ListarClientes(TipoRolCliente rol, EstadoFiltro filtro)
            var inquilinosPRC = cnPRC.ListarClientes(TipoRolCliente.Inquilino, EstadoFiltro.Activos);

            var dsInquilinos = inquilinosPRC
                .Where(x => x.oPersona != null)
                .Select(x => new
                {
                    Dni = x.Dni,
                    DisplayText = $"{x.oPersona.Apellido}, {x.oPersona.Nombre} ({x.Dni})"
                })
                .ToList();

            comboBox2.ValueMember = "Dni";
            comboBox2.DisplayMember = "DisplayText";
            comboBox2.DataSource = dsInquilinos;

            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
        }

        // === Helpers de validación y fecha ===

        private bool TryGetInt(TextBox tb, string campo, out int valor)
        {
            if (!int.TryParse(tb.Text.Trim(), out valor) || valor <= 0)
            {
                MessageBox.Show($"Ingrese un valor válido para {campo}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Focus();
                return false;
            }
            return true;
        }

        private bool TryGetDecimal(TextBox tb, string campo, out decimal valor)
        {
            if (!decimal.TryParse(tb.Text.Trim(), out valor) || valor < 0)
            {
                MessageBox.Show($"Ingrese un valor decimal válido para {campo}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Focus();
                return false;
            }
            return true;
        }

        private void DateInputs_Changed(object sender, EventArgs e)
        {
            CalcularFechaFin();
        }

        private void CalcularFechaFin()
        {
            if (!int.TryParse(TNacimiento.Text.Trim(), out var cant) || cant <= 0)
                return;

            var inicio = dateTimePicker_Inicio.Value.Date;
            var fin = inicio.AddMonths(cant);
            // último día del mes final
            var ultimo = DateTime.DaysInMonth(fin.Year, fin.Month);
            dateTimePicker_fin.Value = new DateTime(fin.Year, fin.Month, ultimo);
        }

        // === Botones ===

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones básicas
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un inmueble.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (comboBox2.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un inquilino.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!TryGetInt(TNacimiento, "Cantidad de cuotas", out var cantidadCuotas))
                    return;
                if (!TryGetDecimal(TNombre, "Precio de cuota", out var precioCuota))
                    return;

                var tasa = numericUpDown_TasaMensual.Value; // %
                var fechaInicio = dateTimePicker_Inicio.Value.Date;
                var fechaFin = dateTimePicker_fin.Value.Date;



                // Construir entidad
                var contrato = new ContratoAlquiler
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Condiciones = textBox_Condiciones.Text ?? string.Empty,
                    CantidadCuotas = cantidadCuotas,
                    PrecioCuota = precioCuota,
                    FechaCreacion = DateTime.Now,
                    InmuebleId = Convert.ToInt32(comboBox1.SelectedValue), // IdInmueble
                    DniInquilino = Convert.ToString(comboBox2.SelectedValue),
                    RolInquilinoId = 2,                       // ajusta según tu catálogo
                    TasaMoraMensual = (decimal)tasa,
                    Estado = 1                       // Vigente
            };

                //asignamos usuario creador
                if (SesionUsuario.UsuarioActual != null)
                {
                    contrato.UsuarioCreador = SesionUsuario.UsuarioActual.IdUsuario;
                }


                // Registrar
                var cnContrato = new CN_Contrato();
                bool ok = cnContrato.RegistrarContrato(contrato, out int nuevoId, out string msg);

                if (ok)
                {
                    MessageBox.Show($"Contrato creado (ID: {nuevoId}).",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"No se pudo crear el contrato.\n{msg}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al crear el contrato: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
