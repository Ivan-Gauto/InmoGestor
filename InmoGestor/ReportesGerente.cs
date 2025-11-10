using CapaEntidades;
using CapaNegocio;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace InmoGestor
{
    public partial class ReportesGerente : Form
    {
        private CN_Reportes objCN_Reportes = new CN_Reportes();

        public ReportesGerente()
        {
            InitializeComponent();

            // Recalcular automáticamente al tocar fechas
            dateTimePicker1.ValueChanged += (_, __) => CargarDatosReporte();
            dateTimePicker2.ValueChanged += (_, __) => CargarDatosReporte();

            // Algunos DTP solo disparan al cerrar el calendario:
            dateTimePicker1.CloseUp += (_, __) => CargarDatosReporte();
            dateTimePicker2.CloseUp += (_, __) => CargarDatosReporte();
        }

        // Tu función de descarga (está perfecta)
        private void Descargarcvs(DataGridView dgv)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos CSV (*.csv)|*.csv";
            sfd.FileName = "Reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // Encabezados
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        sb.Append(dgv.Columns[i].HeaderText + ";");
                    }
                    sb.Append("\r\n");

                    // Filas
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                string valor = row.Cells[i].Value != null ? row.Cells[i].Value.ToString() : "";
                                valor = valor.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                                sb.Append(valor + ";");
                            }
                            sb.Append("\r\n");
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Reporte exportado exitosamente.", "Exportación CSV Exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Evento del ComboBox (MODIFICADO)
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            string reporteSeleccionado = comboBox2.SelectedItem.ToString();

            // Lógica de visibilidad
            dgvRendimientoOperadores.Visible = (reporteSeleccionado == "Rendimiento de Operadores");
            dgvOcupacionYVacancia.Visible = (reporteSeleccionado == "Ocupación y Vacancia de inmuebles");

            CargarDatosReporte();
        }

        // Evento Load del Formulario (MODIFICADO)

        private void ReportesGerente_Load(object sender, EventArgs e)
        {
            ConfigurarGrids();

            // Suscribir cambios de fecha -> recarga automática
            dateTimePicker1.ValueChanged += dateTime_ValueChanged;
            dateTimePicker2.ValueChanged += dateTime_ValueChanged;

            comboBox2.SelectedIndex = 0; // carga inicial
        }

        private void dateTime_ValueChanged(object sender, EventArgs e)
        {
            // (opcional) evitar consultas inválidas
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date) return;

            CargarDatosReporte();
        }

        // Evento del Botón Descargar (MODIFICADO)
        private void BDescargarcvs_Click(object sender, EventArgs e)
        {
            if (dgvRendimientoOperadores.Visible)
            {
                Descargarcvs(dgvRendimientoOperadores);
            }
            else if (dgvOcupacionYVacancia.Visible)
            {
                Descargarcvs(dgvOcupacionYVacancia);
            }
            else
            {
                MessageBox.Show("No hay datos para exportar.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- MÉTODOS DE LÓGICA (AÑADIDOS) ---

        private void CargarDatosReporte()
        {
            if (comboBox2.SelectedItem == null) return;

            var fechaInicio = dateTimePicker1.Value.Date;
            var fechaFin = dateTimePicker2.Value.Date;

            // ✅ Validación: inicio no puede ser mayor que fin
            if (fechaInicio > fechaFin)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha de fin.",
                    "Rango de fechas inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reporteSeleccionado = comboBox2.SelectedItem.ToString();
            fechaInicio = dateTimePicker1.Value;
            fechaFin = dateTimePicker2.Value;



            // --- LÓGICA DE CARGA (Ajustada a tu Designer) ---
            if (reporteSeleccionado == "Rendimiento de Operadores")
            {
                DataTable dtRendimiento = objCN_Reportes.ObtenerRendimientoOperadores(fechaInicio, fechaFin);
                dgvRendimientoOperadores.DataSource = dtRendimiento;
            }
            else if (reporteSeleccionado == "Ocupación y Vacancia de inmuebles")
            {
                DataTable dtOcupacion = objCN_Reportes.ObtenerReporteOcupacion();
                dgvOcupacionYVacancia.DataSource = dtOcupacion;
            }
        }

        // Asigna los DataPropertyName (¡IMPORTANTE!)
        private void ConfigurarGrids()
        {
            // --- Configuración Grid Rendimiento Operadores ---
            dgvRendimientoOperadores.AutoGenerateColumns = false;
            // Conecta tus columnas del designer con la consulta SQL
            ColumnaOperador.DataPropertyName = "Operador";
            ColumnaContratosNuevos.DataPropertyName = "ContratosNuevos";
            ColumnaContratosRenovados.DataPropertyName = "ContratosRenovados";
            ColumnaIngresos.DataPropertyName = "IngresosTotalesGestionados";
            ColumnaDeuda.DataPropertyName = "MontoDeudaTotal";

            // --- Configuración Grid Ocupación ---
            dgvOcupacionYVacancia.AutoGenerateColumns = false;
            // Conecta tus columnas del designer con la consulta SQL
            ColumnaInmueble.DataPropertyName = "Inmueble";
            ColumnaTipoInmueble.DataPropertyName = "Tipo";
            ColumnaDiasOcupados.DataPropertyName = "DiasOcupados";
            ColumnaDiasVacantes.DataPropertyName = "DiasVacantes";
            ColumnaPorcentajeOcupacion.DataPropertyName = "PorcentajeOcupacion";
        }

        // Refresca el reporte si cambian las fechas
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarDatosReporte();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            CargarDatosReporte();
        }

        // --- MÉTODO OPCIONAL: PINTAR CELDAS ---
        // Conecta esto al evento CellFormatting de 'dgvOcupacionYVacancia'
        private void dgvOcupacionYVacancia_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Pintar la celda de % Ocupación según el valor
            if (dgvOcupacionYVacancia.Columns[e.ColumnIndex].Name == "ColumnaPorcentajeOcupacion")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal porcentaje))
                {
                    if (porcentaje < 50.0m) // Menos del 50%
                    {
                        e.CellStyle.BackColor = Color.Firebrick;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else if (porcentaje < 80.0m) // Menos del 80%
                    {
                        e.CellStyle.BackColor = Color.Goldenrod;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else // 80% o más
                    {
                        e.CellStyle.BackColor = Color.DarkGreen;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    // Formatear como porcentaje
                    e.Value = $"{porcentaje:F2} %";
                    e.FormattingApplied = true;
                }
            }
        }
    }
}