using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;
using CapaEntidades;

namespace InmoGestor
{
    public partial class Pagos : Form
    {
        private RegistrarPago registrarPagoForm;

        // Capa de negocio y cache local
        private readonly CN_Pago _cnPago = new CN_Pago();
        private List<Pago> _pagosData = new List<Pago>();

        // (Opcional) mapear id de método de pago a nombre; por ahora mostramos "Método #id"
        private readonly Dictionary<int, string> _metodosPago = new Dictionary<int, string>();

        // Rol actual del usuario logueado (2=Gerente, 3=Operador)
        public int RolUsuarioActual { get; set; } = Roles.Operador;

        public Pagos()
        {
            InitializeComponent();
            this.Resize += Pagos_Resize;
        }

        // Si preferís pasar el rol explícito:
        public Pagos(int rolUsuario) : this()
        {
            RolUsuarioActual = rolUsuario;
        }

        // Helpers para forms hijos
        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() => IsOpen(registrarPagoForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(registrarPagoForm)) { registrarPagoForm.BringToFront(); registrarPagoForm.Focus(); return; }
        }

        private void Pagos_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(registrarPagoForm) ? (Form)registrarPagoForm : null;
            if (f == null) return;

            f.Location = new Point(
                (ContenedorPagos.Width - f.Width) / 2,
                (ContenedorPagos.Height - f.Height) / 2
            );
        }

        private void Pagos_Load(object sender, EventArgs e)
        {
            // Config básica del grid (las columnas ya están en el Designer)
            dataGridPagos.AutoGenerateColumns = false;
            dataGridPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridPagos.MultiSelect = false;
            dataGridPagos.ReadOnly = true;
            dataGridPagos.AllowUserToAddRows = false;

            // Eventos para botones en el grid
            dataGridPagos.CellContentClick += dataGridPagos_CellContentClick;

            CargarDatos();
        }

        // === CARGA Y PINTADO ===

        private void CargarDatos()
        {
            try
            {
                _pagosData = _cnPago.ListarTodos();
                PintarGrilla(_pagosData);
                RefrescarContadores(_pagosData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pagos: " + ex.Message, "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EstadoDesc(int e)
        {
            switch (e)
            {
                case 0: return "Anulado";
                case 1: return "Confirmado";
                case 2: return "Solicitud";
                default: return "Desconocido";
            }
        }

        private void PintarGrilla(List<Pago> pagos)
        {
            var ar = new CultureInfo("es-AR");
            dataGridPagos.Rows.Clear();

            foreach (var p in pagos)
            {
                var nroComprobante = "-"; // si luego unís con RECIBO, completar
                var periodo = p.Periodo ?? "-";
                var fechaPago = p.FechaPago.ToString("dd/MM/yyyy");
                var adicionalMora = p.MoraCobrada;
                var metodo = _metodosPago.TryGetValue(p.MetodoPagoId, out var nombreMetodo)
                                ? nombreMetodo
                                : $"Método #{p.MetodoPagoId}";
                var otrosAdicionales = "-"; // no está en la tabla pago
                var montoPagado = p.MontoTotal;
                var montoCuota = Math.Max(0, p.MontoTotal - p.MoraCobrada); // “monto base” aproximado
                var estado = EstadoDesc(p.Estado);

                // Orden debe coincidir con las columnas del Designer:
                // ColumnaFin, ColumnaPeriodo, ColumnaFechaPago, ColumnaMontoCuota,
                // ColumnaAdicionalMora, ColumnaInicio, ColumnaAdicionalOtros,
                // ColumnaMonto, ColumnaEstado, ColumnaAnularPago, ColumnaConfirmar
                int rowIndex = dataGridPagos.Rows.Add(
                    nroComprobante,
                    periodo,
                    fechaPago,
                    montoCuota.ToString("C0", ar),
                    adicionalMora.ToString("C0", ar),
                    metodo,
                    otrosAdicionales,
                    montoPagado.ToString("C0", ar),
                    estado,
                    null,   // imagen set en Designer (Anular)
                    null    // imagen set en Designer (Confirmar) si corresponde
                );

                // Guardamos el pago_id para acciones (Confirmar/Anular)
                dataGridPagos.Rows[rowIndex].Tag = p.PagoId;
            }
        }

        private void RefrescarContadores(List<Pago> pagos)
        {
            var ar = new CultureInfo("es-AR");

            decimal totalRecaudado = 0m;
            int confirmados = 0;
            int anulados = 0;

            foreach (var p in pagos)
            {
                if (p.Estado == 1) { confirmados++; totalRecaudado += p.MontoTotal; }
                else if (p.Estado == 0) { anulados++; }
            }

            label11.Text = totalRecaudado.ToString("C0", ar); // Total recaudado
            label4.Text = confirmados.ToString();             // Pagos confirmados
            label6.Text = anulados.ToString();                // Anulados
        }

        // Permite refrescar desde otros formularios (ej: al cerrar RegistrarPago)
        public void RefrescarListado()
        {
            CargarDatos();
        }

        // === BOTONES EN GRID (Confirmar / Anular) ===

        private void dataGridPagos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Click fuera de rango
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = dataGridPagos;

            // Columnas del diseñador
            var isColAnular = grid.Columns[e.ColumnIndex].Name == "ColumnaAnularPago";
            var isColConfirmar = grid.Columns[e.ColumnIndex].Name == "ColumnaConfirmar";

            if (!isColAnular && !isColConfirmar) return;

            // Recuperar pago_id guardado en la fila
            var row = grid.Rows[e.RowIndex];
            if (row.Tag == null)
            {
                MessageBox.Show("No se pudo identificar el pago seleccionado.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long pagoId;
            try { pagoId = Convert.ToInt64(row.Tag); }
            catch
            {
                MessageBox.Show("Identificador de pago inválido.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (isColConfirmar)
                {
                    // Confirmar: permitido para  Gerente (2)
                    var ok = _cnPago.ConfirmarPago(pagoId, RolUsuarioActual);
                    if (ok)
                    {
                        MessageBox.Show("Pago confirmado correctamente.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefrescarListado();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo confirmar el pago.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (isColAnular)
                {
                    // Anular: SOLO Gerente (2)
                    var ok = _cnPago.AnularPago(pagoId, RolUsuarioActual);
                    if (ok)
                    {
                        MessageBox.Show("Pago anulado correctamente.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefrescarListado();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo anular el pago.", "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Caso típico: operador intentando anular
                MessageBox.Show(ex.Message, "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                // Reglas de negocio: estados inválidos, etc.
                MessageBox.Show(ex.Message, "Acción no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al ejecutar la acción: " + ex.Message, "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // === BOTÓN REGISTRAR ===

        private void BRegistrarPago_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            registrarPagoForm = new RegistrarPago
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };

            // Si querés refrescar automáticamente al cerrar RegistrarPago:
            registrarPagoForm.FormClosed += (_, __) =>
            {
                ContenedorPagos.Controls.Remove(registrarPagoForm);
                registrarPagoForm.Dispose();
                registrarPagoForm = null;
                RefrescarListado(); // recargar datos y contadores
            };

            ContenedorPagos.Controls.Add(registrarPagoForm);
            registrarPagoForm.Show();
            registrarPagoForm.BringToFront();
            registrarPagoForm.Focus();
            CentrarFormAbierto();
        }
    }
}
