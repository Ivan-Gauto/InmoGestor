using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaNegocio;
// using CapaEntidades; // solo si necesitás tipos del DTO

namespace InmoGestor
{
    public partial class Contratos : Form
    {
        private AgregarContrato agregarContratoForm;

        public Contratos()
        {
            InitializeComponent();
            this.Resize += Contratos_Resize;
        }

        // ================== Helpers de ventana embebida ==================
        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;

        private bool HayFormAbierto() =>
            IsOpen(agregarContratoForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarContratoForm)) { agregarContratoForm.BringToFront(); agregarContratoForm.Focus(); return; }
        }

        private void Contratos_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarContratoForm) ? (Form)agregarContratoForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorContratos.Width - f.Width) / 2,
                (ContenedorContratos.Height - f.Height) / 2
            );
        }

        // ================== Alta de contrato (panel embebido) ==================
        private void BAgregarContrato_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarContratoForm = new AgregarContrato
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };

            ContenedorContratos.Controls.Add(agregarContratoForm);

            // Importante: parámetros con nombre (evita el error de tuplas) y dentro de un método
            agregarContratoForm.FormClosed += (object s, FormClosedEventArgs args) =>
            {
                ContenedorContratos.Controls.Remove(agregarContratoForm);
                agregarContratoForm.Dispose();
                agregarContratoForm = null;
                // refrescamos listado y KPIs al cerrar (por si se creó contrato)
                CargarContratosYKpis();
            };

            agregarContratoForm.Show();
            agregarContratoForm.BringToFront();
            agregarContratoForm.Focus();
            CentrarFormAbierto();
        }

        // ================== Carga inicial ==================
        private void Contratos_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            // El Designer ya engancha CellFormatting? Si no, descomentá la línea de abajo:
            // this.dataGridContratos.CellFormatting += dataGridContratos_CellFormatting;

            // Carga inicial: solo vigentes (1). Para todos, pasá null.
            CargarContratosYKpis(1);
        }

        // ================== Grid: columnas -> propiedades del DTO ==================
        private void ConfigurarGrid()
        {
            dataGridContratos.AutoGenerateColumns = false;

            // --- NOMBRES DE PROPIEDAD CORREGIDOS ---
            // Variable de tu Columna -> Propiedad de ContratoGridRow
            ColumnaInquilino.DataPropertyName = "Inquilino";
            ColumnaDireccion.DataPropertyName = "Direccion";
            ColumnaPrecioCuotas.DataPropertyName = "PrecioCuota";
            ColumnaCuotas.DataPropertyName = "CantCuotas";      // <-- Corregido
            ColumnaInicio.DataPropertyName = "FechaInicio";    // <-- Corregido
            ColumnaFin.DataPropertyName = "FechaFin";          // <-- Corregido
            ColumnaPorcentajeAumentoMora.DataPropertyName = "MoraDiaria"; // <-- Corregido

            // Estilos (Tu código original está bien)
            ColumnaPrecioCuotas.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            ColumnaCuotas.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnaInicio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnaFin.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnaPorcentajeAumentoMora.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // ================== Carga de datos ==================
        private void CargarContratos(int? estado = null)
        {
            var cn = new CN_Contrato();
            var filas = cn.ListarParaGrid(estado);     // List<ContratoGridRow> con las props mapeadas arriba
            dataGridContratos.DataSource = filas;
        }

        private void CargarKpis()
        {
            var cn = new CN_Contrato();
            var (total, activos, porVencer) = cn.ObtenerKpis(30); // ← deconstrucción

            label_CantContratos.Text = total.ToString();
            label_CantActivos.Text = activos.ToString();
            label_CantXVencer.Text = porVencer.ToString();
        }

        private void CargarContratosYKpis(int? estado = null)
        {
            CargarContratos(estado);
            CargarKpis();
        }

        // ================== Formateo visual ==================
        private void dataGridContratos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;
            var colName = dataGridContratos.Columns[e.ColumnIndex].Name;

            if (colName == nameof(ColumnaPrecioCuotas))
            {
                if (e.Value is decimal dec) e.Value = dec.ToString("C"); // moneda local
            }
            else if (colName == nameof(ColumnaInicio) || colName == nameof(ColumnaFin))
            {
                if (e.Value is DateTime dt) e.Value = dt.ToString("dd/MM/yyyy");
            }
            else if (colName == nameof(ColumnaPorcentajeAumentoMora))
            {
                // Si viene como 0.15 (15), mostrás "15 %"
                if (e.Value is decimal p) e.Value = p.ToString("0.##' %'");
            }
        }

        // ================== Click en celdas (Rescindir) ==================
        private void dataGridContratos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Asumo que tu columna se llama 'ColumnaRescindir'
            var clickedCol = dataGridContratos.Columns[e.ColumnIndex].Name;
            if (clickedCol != "ColumnaRescindir") return;

            var rowData = dataGridContratos.Rows[e.RowIndex].DataBoundItem;
            if (rowData == null) return;

            // --- CORRECCIÓN AQUÍ ---
            // La propiedad se llama 'Id', no 'ContratoId'
            int contratoId = (int)rowData.GetType().GetProperty("Id").GetValue(rowData, null);

            var r = MessageBox.Show("¿Rescindir (anular) el contrato seleccionado?",
                                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;

            var cn = new CN_Contrato();
            if (cn.AnularContrato(contratoId, out string msg))
            {
                MessageBox.Show("Contrato anulado correctamente.", "OK",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarContratosYKpis(); // refrescar grilla y KPIs
            }
            else
            {
                MessageBox.Show($"No se pudo anular.\n{msg}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
