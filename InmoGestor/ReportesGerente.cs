using System;
using System.Data;
using System.Drawing; // Asegúrate de tener este using
using System.IO;
using System.Text;
using System.Windows.Forms;
using CapaNegocio;

namespace InmoGestor
{
    public partial class ReportesGerente : Form
    {
        private CN_Reportes objCN_Reportes = new CN_Reportes();

        // Asumo que tienes estos controles de fecha
        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;

        public ReportesGerente()
        {
            InitializeComponent();
            // Asignación manual de controles de fecha (ajusta los nombres)
            dtpFechaInicio = dateTimePicker1;
            dtpFechaFin = dateTimePicker2;
        }

        // Tu función de descarga (simplificada)
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
            string reporteSeleccionado = comboBox2.SelectedItem.ToString();

            if (reporteSeleccionado == "Resumen de Transacciones")
            {
                dgvOcupacionYVacancia.Visible = true; // <-- NOMBRE CORREGIDO
                dgvRendimientoOperadores.Visible = false;
            }
            else if (reporteSeleccionado == "Rendimiento de Operadores")
            {
                dgvOcupacionYVacancia.Visible = false; // <-- NOMBRE CORREGIDO
                dgvRendimientoOperadores.Visible = true;
            }

            CargarDatosReporte();
        }

        // Evento Load del Formulario (MODIFICADO)
        private void ReportesGerente_Load(object sender, EventArgs e)
        {
            ConfigurarGrids();

            comboBox2.Items.Clear();
            comboBox2.Items.Add("Resumen de Transacciones"); // <-- MODIFICADO
            comboBox2.Items.Add("Rendimiento de Operadores");

            comboBox2.SelectedIndex = 0;
        }

        // Evento del Botón Descargar (MODIFICADO)
        private void BDescargarcvs_Click(object sender, EventArgs e)
        {
            if (dgvOcupacionYVacancia.Visible) // <-- NOMBRE CORREGIDO
            {
                Descargarcvs(dgvOcupacionYVacancia); // <-- NOMBRE CORREGIDO
            }
            else if (dgvRendimientoOperadores.Visible)
            {
                Descargarcvs(dgvRendimientoOperadores);
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

            string reporteSeleccionado = comboBox2.SelectedItem.ToString();
            DateTime fechaInicio = dtpFechaInicio.Value;
            DateTime fechaFin = dtpFechaFin.Value;

            if (reporteSeleccionado == "Resumen de Transacciones")
            {
                DataTable dtTransacciones = objCN_Reportes.ObtenerResumenTransacciones(fechaInicio, fechaFin);
                dgvOcupacionYVacancia.DataSource = dtTransacciones; // <-- NOMBRE CORREGIDO
            }
            else if (reporteSeleccionado == "Rendimiento de Operadores")
            {
                DataTable dtRendimiento = objCN_Reportes.ObtenerRendimientoOperadores(fechaInicio, fechaFin);
                dgvRendimientoOperadores.DataSource = dtRendimiento;
            }
        }

        // Asigna los DataPropertyName
        private void ConfigurarGrids()
        {
            // --- Configuración Grid Resumen Transacciones ---
            dgvOcupacionYVacancia.AutoGenerateColumns = false; // <-- NOMBRE CORREGIDO
            // Ajusta los nombres de tus columnas "Columna..." en el diseñador
            /*
            ColumnaFecha.DataPropertyName = "Fecha";
            ColumnaTipoTransaccion.DataPropertyName = "TipoTransaccion";
            ColumnaOperadorTrans.DataPropertyName = "Operador";
            ColumnaInquilinoTrans.DataPropertyName = "Inquilino";
            ColumnaPeriodoTrans.DataPropertyName = "Periodo";
            ColumnaMetodoTrans.DataPropertyName = "Metodo";
            ColumnaMontoTrans.DataPropertyName = "Monto";
            */

            // --- Configuración Grid Rendimiento Operadores ---
            dgvRendimientoOperadores.AutoGenerateColumns = false;
            // Ajusta los nombres de tus columnas "Columna..."
            /*
            ColumnaNombreOperador.DataPropertyName = "Operador";
            ColumnaPagosRegistrados.DataPropertyName = "PagosRegistrados";
            ColumnaMontoRegistrado.DataPropertyName = "MontoTotalRegistrado";
            ColumnaAnulaciones.DataPropertyName = "AnulacionesRealizadas";
            */
        }

        // Refresca el reporte si cambian las fechas
        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {
            CargarDatosReporte();
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            CargarDatosReporte();
        }

        // --- MÉTODO OPCIONAL: PINTAR FILAS ---
        // Conecta esto al evento CellFormatting de 'dgvTransacciones'
        private void dgvTransacciones_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Asumo que la columna se llama "ColumnaTipoTransaccion"
            string tipoTransaccion = dgvOcupacionYVacancia.Rows[e.RowIndex].Cells["ColumnaTipoTransaccion"].Value?.ToString();

            if (tipoTransaccion == "Egreso (Anulación)")
            {
                e.CellStyle.BackColor = Color.DarkOrange;
                e.CellStyle.ForeColor = Color.White;
            }
            else
            {
                e.CellStyle.BackColor = Color.FromArgb(15, 30, 45); // Tu color de fondo
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }
}