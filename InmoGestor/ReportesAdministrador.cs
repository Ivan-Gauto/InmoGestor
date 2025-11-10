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
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class ReportesAdministrador : Form
    {
        // Instancias de Capa de Negocio
        private CN_Reportes negocioReportes = new CN_Reportes();
        private CN_Usuario negocioUsuario = new CN_Usuario();

        private List<LogUsuario> _listaLogUsuarios = new List<LogUsuario>();

        public ReportesAdministrador()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.ReportesAdministrador_Load);
            this.CBTipoReporte.SelectedIndexChanged += new System.EventHandler(this.CBTipoReporte_SelectedIndexChanged);
            this.CBUsuarios.SelectedIndexChanged += new System.EventHandler(this.Filtro_SelectedIndexChanged);
            this.dtpFechaInicio.ValueChanged += new System.EventHandler(this.Filtro_DateValueChanged);
            this.dtpFechaFin.ValueChanged += new System.EventHandler(this.Filtro_DateValueChanged);

            dgvAuditoriaUsuarios.AutoGenerateColumns = false;
            dgvErroresSistema.AutoGenerateColumns = false;

            // --- AÑADIDO ---
            // Conectar el evento de formato de celda para los errores
            this.dgvErroresSistema.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvErroresSistema_CellFormatting);
        }

        private void Filtro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
            {
                GenerarReporte(sender, e);
            }
        }

        private void Filtro_DateValueChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
            {
                GenerarReporte(sender, e);
            }
        }

        private void GenerarReporte(object sender, EventArgs e)
        {
            if (CBTipoReporte.SelectedItem == null) return;
            string tipoReporte = CBTipoReporte.SelectedItem.ToString();
            string usuarioDniSeleccionado = (CBUsuarios.SelectedValue?.ToString() == "0") ? null : CBUsuarios.SelectedValue?.ToString();
            DateTime fechaInicio = dtpFechaInicio.Value.Date;
            DateTime fechaFin = dtpFechaFin.Value.Date.AddDays(1).AddSeconds(-1);

            if (tipoReporte == "Auditoría de Usuarios")
            {
                CargarReporteAuditoria(usuarioDniSeleccionado, fechaInicio, fechaFin);
            }
            else if (tipoReporte == "Monitor de Errores")
            {
                CargarReporteErrores(fechaInicio, fechaFin);
            }
        }

        private void ReportesAdministrador_Load(object sender, EventArgs e)
        {
            ConfigurarGrids();

            CBTipoReporte.Items.Add("Auditoría de Usuarios");
            CBTipoReporte.Items.Add("Monitor de Errores");

            dtpFechaInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFechaFin.Value = DateTime.Now;

            CargarUsuarios();

            CBTipoReporte.SelectedIndex = 0;
        }

        private void CargarUsuarios()
        {
            List<Usuario> listaUsuarios = negocioUsuario.Listar(RolUsuarioFiltro.Administradores, EstadoFiltro.Activos);

            var usuarioTodos = new Usuario();
            usuarioTodos.Dni = "0";
            usuarioTodos.oPersona.Nombre = "Todos";
            usuarioTodos.oPersona.Apellido = "";
            listaUsuarios.Insert(0, usuarioTodos);

            CBUsuarios.DataSource = listaUsuarios;
            CBUsuarios.DisplayMember = "NombreCompleto";
            CBUsuarios.ValueMember = "Dni";
            CBUsuarios.SelectedIndex = 0;
        }

        private void CBTipoReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBTipoReporte.SelectedItem == null) return;
            string seleccion = CBTipoReporte.SelectedItem.ToString();

            bool esAuditoria = (seleccion == "Auditoría de Usuarios");

            dgvAuditoriaUsuarios.Visible = esAuditoria;
            dgvErroresSistema.Visible = !esAuditoria;

            CBUsuarios.Visible = esAuditoria;
            label3.Visible = esAuditoria;

            GenerarReporte(sender, e);
        }

        private void BExportarCSV_Click(object sender, EventArgs e)
        {
            DataGridView dgvVisible = null;

            if (dgvAuditoriaUsuarios.Visible)
            {
                dgvVisible = dgvAuditoriaUsuarios;
            }
            else if (dgvErroresSistema.Visible)
            {
                dgvVisible = dgvErroresSistema;
            }

            if (dgvVisible != null && dgvVisible.Rows.Count > 0)
            {
                DescargarGridComoCsv(dgvVisible, CBTipoReporte.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("No hay datos cargados para exportar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DescargarGridComoCsv(DataGridView dgv, string nombreReporte)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos CSV (*.csv)|*.csv";
            saveFileDialog.Title = "Guardar Reporte";
            saveFileDialog.FileName = $"{nombreReporte.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.csv";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        sb.Append(col.HeaderText + ";");
                    }
                    sb.Append("\r\n");

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            string valor = cell.FormattedValue != null ? cell.FormattedValue.ToString() : "";
                            valor = valor.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                            sb.Append(valor + ";");
                        }
                        sb.Append("\r\n");
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Reporte exportado con éxito.", "Exportación Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al exportar: {ex.Message}", "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarReporteAuditoria(string adminDni, DateTime inicio, DateTime fin)
        {
            dgvAuditoriaUsuarios.Rows.Clear();
            _listaLogUsuarios = negocioReportes.ListarAuditoriaUsuarios(adminDni, inicio, fin);

            foreach (var log in _listaLogUsuarios)
            {
                dgvAuditoriaUsuarios.Rows.Add(new object[] {
                    log.AdminEjecutorDni,
                    log.FechaHora.ToString("dd/MM/yyyy HH:mm:ss"),
                    log.UsuarioAfectadoDni,
                    log.Accion,
                });
            }
        }

        // --- MÉTODO MODIFICADO ---
        // Usa DataSource para llenar la grilla de errores
        private void CargarReporteErrores(DateTime inicio, DateTime fin)
        {
            dgvErroresSistema.DataSource = null;
            DataTable dt = negocioReportes.ObtenerLogErrorAgrupado(inicio, fin);
            dgvErroresSistema.DataSource = dt;
        }

        // --- MÉTODO NUEVO ---
        // Asigna los DataPropertyName
        private void ConfigurarGrids()
        {
            dgvAuditoriaUsuarios.AutoGenerateColumns = false;
            // (Aquí va la configuración de dgvAuditoriaUsuarios si la tienes)

            // --- Configuración Grid Errores ---
            dgvErroresSistema.AutoGenerateColumns = false;

            // Conecta las columnas que creaste en el diseñador
            // con la consulta SQL.
            ColumnaCantidad.DataPropertyName = "Cantidad";
            ColumnaUltimaVez.DataPropertyName = "UltimaVez";
            ColumnaFormulario.DataPropertyName = "Formulario";
            ColumnaMetodo.DataPropertyName = "Metodo";
            ColumnaMensajeDeError.DataPropertyName = "MensajeError";
        }

        // --- MÉTODO NUEVO ---
        // Pinta las celdas de 'dgvErroresSistema'
        private void dgvErroresSistema_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Pintar la celda de Cantidad según el valor
            if (dgvErroresSistema.Columns[e.ColumnIndex].Name == "ColumnaCantidad")
            {
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int cantidad))
                {
                    if (cantidad > 20)
                    {
                        e.CellStyle.BackColor = Color.Firebrick;
                    }
                    else if (cantidad > 5)
                    {
                        e.CellStyle.BackColor = Color.Goldenrod;
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.DarkSlateGray; // Color para pocos errores
                    }
                    e.CellStyle.ForeColor = Color.White;
                }
            }

            // Formatear la fecha
            if (dgvErroresSistema.Columns[e.ColumnIndex].Name == "ColumnaUltimaVez")
            {
                if (e.Value != null && e.Value is DateTime)
                {
                    e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy HH:mm");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}