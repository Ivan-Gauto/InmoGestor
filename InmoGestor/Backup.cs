using System;
using System.Data;
using System.Drawing; // Asegúrate de tener este using para Color
using System.Windows.Forms;
using CapaNegocio;

namespace InmoGestor
{
    public partial class Backup : Form
    {
        private CN_Backup objCN_Backup = new CN_Backup();

        public Backup()
        {
            InitializeComponent();
        }

        private void Backup_Load(object sender, EventArgs e)
        {
            ConfigurarColumnasDataGrid();
            dtpFiltrarFecha.Value = DateTime.Now;
            ActualizarVistaCompleta();
        }

        private void BRealizarBackup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos de Backup SQL (*.bak)|*.bak";
                sfd.Title = "Guardar Backup de Base de Datos";
                sfd.FileName = $"inmogestor_db_backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string rutaArchivo = sfd.FileName;
                    string mensajeError = string.Empty;

                    this.Cursor = Cursors.WaitCursor;
                    BRealizarBackup.Enabled = false;

                    bool exito = objCN_Backup.RealizarBackup(rutaArchivo, out mensajeError);

                    this.Cursor = Cursors.Default;
                    BRealizarBackup.Enabled = true;

                    if (exito)
                    {
                        MessageBox.Show("Backup completado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ocurrió un error al generar el backup:\n\n" + mensajeError, "Error de Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    ActualizarVistaCompleta();
                }
            }
        }

        private void dtpFiltrarFecha_ValueChanged(object sender, EventArgs e)
        {
            ActualizarGrilla();
        }

        private void ActualizarVistaCompleta()
        {
            ActualizarTarjetas();
            ActualizarGrilla();
        }

        private void ActualizarTarjetas()
        {
            objCN_Backup.CargarTarjetas(out int total, out int exitosos, out int errores, out decimal espacioUsado);

            LTotalBackups.Text = total.ToString();
            LBackupsExitosos.Text = exitosos.ToString();
            LErrores.Text = errores.ToString();
            LEspacioUsado.Text = $"{espacioUsado:F2} MB";
        }

        // --- MÉTODO MODIFICADO ---
        // Sigue la lógica de Usuarios.cs: Carga un DataTable, limpia la grilla y la llena manualmente.
        private void ActualizarGrilla()
        {
            DateTime fechaFiltro = dtpFiltrarFecha.Value.Date;
            DataTable dt = objCN_Backup.ListarBackups(fechaFiltro);

            dataGridBackups.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                dataGridBackups.Rows.Add(new object[]
                {
                    // El orden debe ser EXACTO al de tus columnas en el diseñador
                    row["nombre_archivo"],
                    row["fecha_hora"],
                    row["tamaño_mb"],
                    row["duracion_segundos"],
                    row["estado"],
                    row["ruta_archivo"],
                    row["mensaje_error"]
                });
            }
        }

        // --- MÉTODO MODIFICADO ---
        // Ya no necesitamos DataPropertyName, solo apagar la autogeneración.
        private void ConfigurarColumnasDataGrid()
        {
            dataGridBackups.AutoGenerateColumns = false;
        }

        // --- NUEVO MÉTODO (Como en Usuarios.cs) ---
        // Se encarga de pintar las filas y formatear el texto.
        private void dataGridBackups_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Obtenemos el estado de la fila actual
            string estado = dataGridBackups.Rows[e.RowIndex].Cells["ColumnaEstado"].Value?.ToString();

            // 1. Pintamos la fila según el estado (igual que en Usuarios.cs)
            if (estado == "Error")
            {
                e.CellStyle.BackColor = Color.DarkOrange;
                e.CellStyle.ForeColor = Color.White;
            }
            else
            {
                // Colores por defecto (asumiendo tema oscuro)
                e.CellStyle.BackColor = Color.FromArgb(15, 30, 45);
                e.CellStyle.ForeColor = Color.White;
            }

            // 2. Formateamos columnas específicas
            string columnName = dataGridBackups.Columns[e.ColumnIndex].Name;

            // Formatear 'Tamaño' para que muestre "MB"
            if (columnName == "ColumnaTamaño" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal tamaño))
                {
                    e.Value = $"{tamaño:F2} MB";
                    e.FormattingApplied = true;
                }
            }

            // Formatear 'Duracion' para que muestre "seg"
            if (columnName == "ColumnaDuracion" && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int duracion))
                {
                    e.Value = $"{duracion} seg";
                    e.FormattingApplied = true;
                }
            }
        }
    }
}