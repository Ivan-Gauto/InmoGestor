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
    public partial class ReportesOperador : Form
    {
        public ReportesOperador()
        {
            InitializeComponent();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Paso 1: Obtener el texto del item seleccionado en el ComboBox.
            string reporteSeleccionado = comboBox2.SelectedItem?.ToString();

            // Si no hay nada seleccionado (raro, pero puede pasar), no hacer nada.
            if (string.IsNullOrEmpty(reporteSeleccionado))
            {
                return;
            }

            // Paso 2: Ocultar TODOS los DataGridViews.
            dgvEstadoDeCuenta.Visible = false;
            dgvInquilinosMorosos.Visible = false;
            dgvInmueblesDisponibles.Visible = false;

            // Aquí puedes añadir más lógica para habilitar/deshabilitar filtros
            // Por ejemplo: txtSeleccionarInquilino.Enabled = true;

            // Paso 3: Usar un switch (o if/else) para decidir qué DataGridView mostrar.
            switch (reporteSeleccionado)
            {
                case "Estado de Cuenta del Inquilino":
                    // Hacemos visible solo la grilla que corresponde.
                    dgvEstadoDeCuenta.Visible = true;
                    // TODO: Llamar al método CargarEstadoDeCuenta() aquí si quieres que se cargue automáticamente
                    break;

                case "Inquilinos Morosos":
                    dgvInquilinosMorosos.Visible = true;
                    // TODO: Llamar al método CargarInquilinosMorosos()
                    break;

                case "Inmuebles Disponibles":
                    dgvInmueblesDisponibles.Visible = true;
                    // TODO: Llamar al método CargarInmueblesDisponibles()
                    break;
            }
        }

        private void ReportesOperador_Load(object sender, EventArgs e)
        {
            // =================================================================
            // CÓDIGO MODIFICADO: Configuración de Reporte por Defecto
            // =================================================================

            // 1. Cargar las opciones disponibles en el ComboBox (si no están ya en el Diseñador)
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Estado de Cuenta del Inquilino");
            comboBox2.Items.Add("Inquilinos Morosos");
            comboBox2.Items.Add("Inmuebles Disponibles");

            // 2. Forzar la selección del primer elemento.
            // Esto dispara el evento comboBox2_SelectedIndexChanged, haciendo visible el primer DataGrid.
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0; // Selecciona "Estado de Cuenta del Inquilino"
            }

            // NOTA: Si necesitas que los datos del reporte se muestren al cargar,
            // la lógica de carga de datos (por ejemplo, al presionar "Generar Vista Previa")
            // debe ser llamada aquí, después de establecer el SelectedIndex.
        }

        private void Descargarcvs(DataGridView dgv)
        {
            // NO se definen constantes para exclusión, ya que no hay botones de acción.

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

                    // 3. Añadir Encabezados (TODAS las columnas visibles)
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        // Usamos ; como separador
                        sb.Append(dgv.Columns[i].HeaderText + ";");
                    }
                    sb.Append("\r\n"); // Salto de línea

                    // 4. Añadir Filas de Datos
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                string valor = row.Cells[i].Value != null ? row.Cells[i].Value.ToString() : "";
                                // Limpiar el valor de ; y saltos de línea
                                valor = valor.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                                sb.Append(valor + ";");
                            }
                            sb.Append("\r\n"); // Salto de línea
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

        private void BDescargarcvs_Click(object sender, EventArgs e)
        {
            // Si el DataGrid de Estado de Cuenta está visible
            if (dgvEstadoDeCuenta.Visible)
            {
                Descargarcvs(dgvEstadoDeCuenta);
            }
            // Si el DataGrid de Inquilinos Morosos está visible
            else if (dgvInquilinosMorosos.Visible)
            {
                Descargarcvs(dgvInquilinosMorosos);
            }
            // Si el DataGrid de Inmuebles Disponibles está visible
            else if (dgvInmueblesDisponibles.Visible)
            {
                Descargarcvs(dgvInmueblesDisponibles);
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un reporte y genere la vista previa para exportar datos.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
