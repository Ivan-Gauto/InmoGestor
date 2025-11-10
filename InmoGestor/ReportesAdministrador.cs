using CapaEntidades;
using CapaNegocio;
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
    public partial class ReportesAdministrador : Form
    {
        // Lista para guardar los resultados del log de auditoría
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
        }

        private void Filtro_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Solo recargar si ya se terminó de cargar la ventana
            if (this.IsHandleCreated)
            {
                GenerarReporte(sender, e);
            }
        }

        // Manejador para los cambios en las Fechas
        private void Filtro_DateValueChanged(object sender, EventArgs e)
        {
            // Solo recargar si ya se terminó de cargar la ventana
            if (this.IsHandleCreated)
            {
                GenerarReporte(sender, e);
            }
        }

        private void GenerarReporte(object sender, EventArgs e)

        {

            // NO exportar CSV aquí. Mostrar el reporte en el DataGrid según selección.

            string tipoReporte = CBTipoReporte.SelectedItem?.ToString() ?? "Auditoría de Usuarios";



            // Obtengo el usuario seleccionado (si es "0" trato como todos -> null)

            string usuarioDniSeleccionado = (CBUsuarios.SelectedValue?.ToString() == "0") ? null : CBUsuarios.SelectedValue?.ToString();



            DateTime fechaInicio = dtpFechaInicio.Value.Date; // Tomar solo la fecha

            DateTime fechaFin = dtpFechaFin.Value.Date.AddDays(1).AddSeconds(-1); // Incluir todo el día final



            if (tipoReporte == "Auditoría de Usuarios")

            {

                CargarReporteAuditoria(usuarioDniSeleccionado, fechaInicio, fechaFin);

            }

            else if (tipoReporte == "Monitor de Errores")

            {

                // Si aún no está implementado, limpiar y avisar

                dgvErroresSistema.Rows.Clear();

                MessageBox.Show("Reporte de Errores aún no implementado.");

            }

        }

        private void ReportesAdministrador_Load(object sender, EventArgs e)
        {
            // Tipos de reporte
            CBTipoReporte.Items.Add("Auditoría de Usuarios");
            CBTipoReporte.Items.Add("Monitor de Errores");
            CBTipoReporte.SelectedIndex = 0;

            // Poner fechas por defecto (ej: inicio de mes hasta hoy)
            dtpFechaInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFechaFin.Value = DateTime.Now;

            // Cargar lista de usuarios antes de pedir el reporte por defecto
            CargarUsuarios();

            // Mostrar un reporte por defecto: todos los usuarios en el rango seleccionado
            string usuarioDniSeleccionado = null; // mostrar para "Todos" por defecto
            DateTime fechaInicio = dtpFechaInicio.Value.Date;
            DateTime fechaFin = dtpFechaFin.Value.Date.AddDays(1).AddSeconds(-1);

            // Cargar directamente en el DataGrid (no exportar CSV)
            CargarReporteAuditoria(usuarioDniSeleccionado, fechaInicio, fechaFin);
        }

        private void CargarUsuarios()
        {
            var negocioUsuario = new CN_Usuario();
            List<Usuario> listaUsuarios = negocioUsuario.Listar(RolUsuarioFiltro.Administradores, EstadoFiltro.Activos);

            var usuarioTodos = new Usuario();
            usuarioTodos.Dni = "0"; // Usar "0" o null para representar "Todos"
            usuarioTodos.oPersona.Nombre = "Todos";
            usuarioTodos.oPersona.Apellido = "";
            listaUsuarios.Insert(0, usuarioTodos);

            CBUsuarios.DataSource = listaUsuarios;
            CBUsuarios.DisplayMember = "NombreCompleto";
            CBUsuarios.ValueMember = "Dni";
            CBUsuarios.SelectedIndex = 0; // Seleccionar "Todos" por defecto
        }

        private void CBTipoReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            string seleccion = CBTipoReporte.SelectedItem.ToString();
            dgvAuditoriaUsuarios.Rows.Clear(); // Limpiar grilla al cambiar tipo
            dgvErroresSistema.Rows.Clear();

            if (seleccion == "Auditoría de Usuarios")
            {
                dgvAuditoriaUsuarios.Visible = true;
                dgvErroresSistema.Visible = false;
            }
            else if (seleccion == "Monitor de Errores")
            {
                dgvAuditoriaUsuarios.Visible = false;
                dgvErroresSistema.Visible = true;
            }
        }

        // Suponiendo que tienes un botón BExportarCSV_Click para la descarga
        private void BExportarCSV_Click(object sender, EventArgs e)
        {
            // Lógica para exportar _listaLogUsuarios a un archivo CSV
            if (_listaLogUsuarios.Any())
            {
                // 1. Mostrar SaveFileDialog
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivos CSV (*.csv)|*.csv";
                saveFileDialog.Title = "Guardar Reporte de Auditoría";
                saveFileDialog.FileName = $"ReporteAuditoria_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 2. Escribir el archivo
                        using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                        {
                            // Encabezados
                            sw.WriteLine("DNI Administrador,Fecha y Hora,DNI Usuario Afectado,Acción");

                            // Datos
                            foreach (var log in _listaLogUsuarios)
                            {
                                sw.WriteLine($"{log.AdminEjecutorDni},{log.FechaHora:dd/MM/yyyy HH:mm:ss},{log.UsuarioAfectadoDni},{log.Accion.Replace(",", "")}"); // Reemplaza comas si la acción las contiene
                            }
                        }

                        MessageBox.Show("Reporte exportado con éxito.", "Exportación Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocurrió un error al exportar: {ex.Message}", "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No hay datos cargados para exportar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CargarReporteAuditoria(string adminDni, DateTime inicio, DateTime fin)
        {
            dgvAuditoriaUsuarios.Rows.Clear();
            var negocioLog = new CN_Log();
            _listaLogUsuarios = negocioLog.ListarAuditoriaUsuarios(adminDni, inicio, fin);

            foreach (var log in _listaLogUsuarios)
            {
                // Asegúrate que los nombres "Columna..." coincidan exactamente
                // con la propiedad 'Name' de tus columnas en el diseñador del DataGridView
                dgvAuditoriaUsuarios.Rows.Add(new object[] {
                    log.AdminEjecutorDni,
                    log.FechaHora.ToString("dd/MM/yyyy HH:mm:ss"), // Formato de fecha/hora
                    log.UsuarioAfectadoDni,
                    log.Accion,
                });
            }

            // Opcional: seleccionar la primera fila si existe
            if (dgvAuditoriaUsuarios.Rows.Count > 0)
            {
                dgvAuditoriaUsuarios.ClearSelection();
                dgvAuditoriaUsuarios.Rows[0].Selected = true;
            }
        }
    }
}