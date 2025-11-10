using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidades; 
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
        // ==== helper: asigna el primer nombre de propiedad que exista en el DTO ====
        private static void BindProp(DataGridViewColumn col, Type rowType, params string[] candidates)
        {
            var props = rowType.GetProperties();
            foreach (var c in candidates)
            {
                if (props.Any(p => string.Equals(p.Name, c, StringComparison.OrdinalIgnoreCase)))
                {
                    col.DataPropertyName = c;
                    return;
                }
            }
            // si no encontró nada, deja el DataPropertyName vacío (columna quedará en blanco)
        }


        // ==== ConfigurarGrid: solo estilos y AutoGenerate ====
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

        // ==== CargarContratos: primero obtengo filas, después bindeo nombres seguros ====
        private void CargarContratos(int? estado = null)
        {
            var cn = new CN_Contrato();
            var filas = cn.ListarParaGrid(estado);

            // si no hay filas, igual intento bindear contra el tipo del DTO
            var rowType = filas?.FirstOrDefault()?.GetType();
            if (rowType != null)
            {
                // ID
                BindProp(ColumnaId, rowType, "Id", "ContratoId");

                // Inquilino
                BindProp(ColumnaInquilino, rowType, "Inquilino", "InquilinoNombre", "InquilinoEtiqueta");

                // Dirección del inmueble
                BindProp(ColumnaDireccion, rowType, "Direccion", "DireccionInmueble");

                // Inmueble (etiqueta)
                BindProp(ColumnaInmueble, rowType, "Inmueble", "InmuebleEtiqueta", "Unidad");

                // Precio de cuota
                BindProp(ColumnaPrecioCuotas, rowType, "PrecioCuota", "MontoCuota", "ImporteBase");

                // Cantidad de cuotas
                BindProp(ColumnaCuotas, rowType, "CantCuotas", "Cuotas");

                // Fechas
                BindProp(ColumnaInicio, rowType, "FechaInicio", "Inicio");
                BindProp(ColumnaFin, rowType, "FechaFin", "Fin");

                // Mora diaria (%)
                BindProp(ColumnaPorcentajeAumentoMora, rowType, "MoraDiariaPct", "MoraDiaria", "PorcentajeMora");
            }

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


        private static decimal GetDec(object obj, string prop)
{
    var p = obj.GetType().GetProperty(prop);
    if (p == null) return 0m;
    var v = p.GetValue(obj);
    if (v is decimal dd) return dd;
    if (v is double dbl) return (decimal)dbl;
    if (v is float fl) return (decimal)fl;
    decimal.TryParse(v?.ToString(), out var res);
    return res;
}



        // ================== Formateo visual ==================
        private void dataGridContratos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var colName = dataGridContratos.Columns[e.ColumnIndex].Name;
            var rowObj = dataGridContratos.Rows[e.RowIndex].DataBoundItem;
            if (rowObj == null) return;

            // Formato $ en precio de cuota
            if (colName == nameof(ColumnaPrecioCuotas))
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out var dec))
                {
                    e.Value = dec.ToString("C0", new System.Globalization.CultureInfo("es-AR"));
                    e.FormattingApplied = true;
                }
                return;
            }

            // Fechas
            if (colName == nameof(ColumnaInicio) || colName == nameof(ColumnaFin))
            {
                if (e.Value is DateTime dt)
                {
                    e.Value = dt.ToString("dd/MM/yyyy");
                    e.FormattingApplied = true;
                }
                return;
            }



            // ===== MORA DIARIA ($) calculada desde MOR A MENSUAL % =====
            if (colName == nameof(ColumnaPorcentajeAumentoMora))
            {
                // 1) Precio de cuota
                decimal precioCuota =
                    GetDec(rowObj, "PrecioCuota") != 0m ? GetDec(rowObj, "PrecioCuota")
                    : GetDec(rowObj, "MontoCuota") != 0m ? GetDec(rowObj, "MontoCuota")
                    : GetDec(rowObj, "ImporteBase");

                // 2) Porcentaje de mora mensual (probar varios nombres)
                decimal moraMensualPct =
                    GetDec(rowObj, "MoraMensual") != 0m ? GetDec(rowObj, "MoraMensual")
                    : GetDec(rowObj, "MoraMensualPct") != 0m ? GetDec(rowObj, "MoraMensualPct")
                    : GetDec(rowObj, "PorcentajeMoraMensual");

                // si no vino mensual, quizá ya tenés diario en %; lo convertimos a mensual para uniformar
                if (moraMensualPct == 0m)
                {
                    var moraDiariaPctProp = GetDec(rowObj, "MoraDiaria") != 0m ? GetDec(rowObj, "MoraDiaria")
                                          : GetDec(rowObj, "MoraDiariaPct");
                    if (moraDiariaPctProp != 0m) moraMensualPct = moraDiariaPctProp * 30m;
                }

                // Normalizar: 10 => 0.10
                if (moraMensualPct > 1m) moraMensualPct /= 100m;

                // 3) Pct diario y monto diario
                var moraDiariaPct = moraMensualPct / 30m;
                var moraDiariaMonto = precioCuota * moraDiariaPct;

                e.Value = moraDiariaMonto.ToString("C0", new System.Globalization.CultureInfo("es-AR"));
                e.FormattingApplied = true;

                // opcional: título amigable
                ColumnaPorcentajeAumentoMora.HeaderText = "Mora diaria ($)";
                return;
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
