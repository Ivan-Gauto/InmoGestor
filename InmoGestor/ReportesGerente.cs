using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace InmoGestor
{
    public partial class ReportesGerente : Form
    {
        public ReportesGerente()
        {
            InitializeComponent();
        }

        private void Descargarcvs(DataGridView dgv)
        {
            // Definimos los nombres de las columnas de acción que NO queremos exportar
            const string NOMBRE_COLUMNA_APROBAR = "ColumnaAprobar"; // Reemplaza si es diferente
            const string NOMBRE_COLUMNA_RECHAZAR = "ColumnaRechazar"; // Reemplaza si es diferente

            // 1. Configurar el cuadro de diálogo Guardar
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos CSV (*.csv)|*.csv";
            sfd.FileName = "Reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 2. Construir el contenido
                    StringBuilder sb = new StringBuilder();

                    // 3. Añadir Encabezados (Excluyendo columnas de acción)
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        string nombreColumnaActual = dgv.Columns[i].Name;

                        // Excluir si el nombre coincide con las columnas de acción
                        if (nombreColumnaActual != NOMBRE_COLUMNA_APROBAR &&
                            nombreColumnaActual != NOMBRE_COLUMNA_RECHAZAR)
                        {
                            sb.Append(dgv.Columns[i].HeaderText + ";"); // Separador ;
                        }
                    }
                    sb.Append("\r\n"); // Salto de línea

                    // 4. Añadir Filas de Datos (Excluyendo valores de las columnas de acción)
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                string nombreColumnaActual = dgv.Columns[i].Name;

                                // Excluir si el nombre coincide con las columnas de acción
                                if (nombreColumnaActual != NOMBRE_COLUMNA_APROBAR &&
                                    nombreColumnaActual != NOMBRE_COLUMNA_RECHAZAR)
                                {
                                    string valor = row.Cells[i].Value != null ? row.Cells[i].Value.ToString() : "";
                                    // Limpiar el valor de ; y saltos de línea
                                    valor = valor.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                                    sb.Append(valor + ";");
                                }
                            }
                            sb.Append("\r\n"); // Salto de línea para la siguiente fila
                        }
                    }

                    // 5. Guardar el archivo con codificación UTF8
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener el valor seleccionado del ComboBox
            string reporteSeleccionado = comboBox2.SelectedItem.ToString();

            // Lógica de visibilidad
            if (reporteSeleccionado == "Pagos Pendientes de Aprobacion")
            {
                // Mostrar el DataGrid de Pagos y ocultar el de Rendimiento
                dgvPagos.Visible = true;
                dgvRendimientoOperadores.Visible = false;

                // Opcional: Cargar los datos de Pagos Pendientes
            }
            else if (reporteSeleccionado == "Rendimiento de Operadores")
            {
                // Mostrar el DataGrid de Rendimiento y ocultar el de Pagos
                dgvPagos.Visible = false;
                dgvRendimientoOperadores.Visible = true;

                // Opcional: Cargar los datos de Rendimiento
            }
            else
            {
                // Ocultar ambos DataGrids si la opción es inválida o un encabezado (ej. "Seleccione un reporte")
                dgvPagos.Visible = false;
                dgvRendimientoOperadores.Visible = false;
            }
        }

        private void ReportesGerente_Load(object sender, EventArgs e)
        {
            // 1. Cargar las opciones en el ComboBox (si no están cargadas en el diseñador)
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Pagos Pendientes de Aprobacion");
            comboBox2.Items.Add("Rendimiento de Operadores");

            // 2. Establecer el valor por defecto
            comboBox2.SelectedIndex = 0;
            // Esto mostrará "Pagos Pendientes de Aprobacion" por defecto.
        }

        private void BDescargarcvs_Click(object sender, EventArgs e)
        {
            if (dgvPagos.Visible)
            {
                Descargarcvs(dgvPagos);
            }
            else if (dgvRendimientoOperadores.Visible)
            {
                Descargarcvs(dgvRendimientoOperadores);
            }
            else
            {
                MessageBox.Show("No hay datos para exportar. Por favor, seleccione un reporte y genere la vista previa.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
