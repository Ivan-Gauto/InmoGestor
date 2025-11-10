using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class Pagos : Form
    {
        // ====== Servicios de negocio ======
        private readonly CN_Contrato _cnContrato = new CN_Contrato();
        private readonly CN_Cuota _cnCuota = new CN_Cuota();
        private readonly CN_Pago _cnPago = new CN_Pago();

        // ====== Estado ======
        private List<ContratoGridRow> _contratosVigentes = new List<ContratoGridRow>();

        // Rol actual del usuario logueado (2=Gerente, 3=Operador)
        public int RolUsuarioActual { get; set; } = Roles.Operador;

        public Pagos()
        {
            InitializeComponent();
            this.BRegistrarPago.Click += new System.EventHandler(this.BRegistrarPago_Click);
            this.Resize += (_, __) => CentrarFormHijoSiHubiera();
            dataGridPagos.CellContentClick += dataGridPagos_CellContentClick;
            this.Load += Pagos_Load;
        }

        // ==== Load ====
        private void Pagos_Load(object sender, EventArgs e)
        {
            // Config básica del grid
            dataGridPagos.AutoGenerateColumns = false;
            dataGridPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridPagos.MultiSelect = false;
            dataGridPagos.ReadOnly = true;
            dataGridPagos.AllowUserToAddRows = false;

            // Desuscribir por seguridad y volver a suscribir (nombres definitivos del Designer)
            CB_Inquilino.SelectedIndexChanged -= CB_Inquilino_SelectedIndexChanged;
            CB_Propietario.SelectedIndexChanged -= CB_Propietario_SelectedIndexChanged;

            CB_Inquilino.SelectedIndexChanged += CB_Inquilino_SelectedIndexChanged;
            CB_Propietario.SelectedIndexChanged += CB_Propietario_SelectedIndexChanged;

            // --- Columna Imprimir Recibo ---
if (dataGridPagos.Columns["ColumnaImprimir"] == null)
{
    var colImprimir = new DataGridViewButtonColumn();
    colImprimir.Name = "ColumnaImprimir";
    colImprimir.HeaderText = "Recibo";
    colImprimir.Text = "Imprimir";
    colImprimir.UseColumnTextForButtonValue = true;
    colImprimir.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
    dataGridPagos.Columns.Add(colImprimir);
}


            CargarInquilinos();
        }

        // ==== Carga Inquilinos ====
        private void CargarInquilinos()
        {
            try
            {
                // 1 = Vigentes (ajusta si tu método usa otro flag)
                _contratosVigentes = _cnContrato.ListarParaGrid(1) ?? new List<ContratoGridRow>();

                var inquilinos = _contratosVigentes
                    .Select(c => (c.Inquilino ?? "").Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .OrderBy(s => s)
                    .ToList();

                CB_Inquilino.DataSource = inquilinos;

                if (inquilinos.Count > 0)
                {
                    CB_Inquilino.SelectedIndex = 0; // dispara CB_Inquilino_SelectedIndexChanged
                }
                else
                {
                    CB_Inquilino.DataSource = null;
                    CB_Propietario.DataSource = null;
                    LimpiarGrillaYKpis();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar inquilinos: " + ex.Message, "Pagos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                CB_Inquilino.DataSource = null;
                CB_Propietario.DataSource = null;
                LimpiarGrillaYKpis();
            }
        }

        // ==== Al cambiar Inquilino (carga CONTRATOS en CB_Propietario) ====
        private void CB_Inquilino_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var inq = CB_Inquilino.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(inq))
                {
                    CB_Propietario.DataSource = null;
                    LimpiarGrillaYKpis();
                    return;
                }

                var contratosDelInquilino = _contratosVigentes
                    .Where(c => string.Equals((c.Inquilino ?? "").Trim(),
                                              (inq ?? "").Trim(),
                                              StringComparison.InvariantCultureIgnoreCase))
                    .Select(c => new ContratoItem(c))   // wrapper para mostrar en combo
                    .OrderBy(ci => ci.Id)
                    .ToList();

                CB_Propietario.DisplayMember = nameof(ContratoItem.Display);
                CB_Propietario.ValueMember = nameof(ContratoItem.Id);
                CB_Propietario.DataSource = contratosDelInquilino;

                if (contratosDelInquilino.Count > 0)
                    CB_Propietario.SelectedIndex = 0; // dispara CB_Propietario_SelectedIndexChanged
                else
                    LimpiarGrillaYKpis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar contratos: " + ex.Message, "Pagos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CB_Propietario.DataSource = null;
                LimpiarGrillaYKpis();
            }
        }

        // ==== Al cambiar CONTRATO (en CB_Propietario) ====
        private void CB_Propietario_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarPagosDeContratoSeleccionado();
        }

        private void CargarPagosDeContratoSeleccionado()
        {
            dataGridPagos.Rows.Clear();

            var item = CB_Propietario.SelectedItem as ContratoItem;
            if (item == null)
            {
                LimpiarGrillaYKpis();
                return;
            }

            try
            {
                // 1) Cuotas del contrato
                var cuotas = _cnCuota.ListarPorContrato(item.Id) ?? new List<Cuota>();

                // 2) Armar filas a partir de (cuota + pagos)
                var filas = new List<PagoRow>();
                foreach (var q in cuotas)
                {
                    var pagos = _cnPago.ListarPorCuota(q.CuotaId) ?? new List<Pago>();
                    foreach (var p in pagos)
                    {
                        filas.Add(new PagoRow
                        {
                            PagoId = p.PagoId,
                            Periodo = q.Periodo ?? "",
                            FechaPago = p.FechaPago,
                            ImporteBase = q.ImporteBase,
                            Mora = p.MoraCobrada,
                            MetodoPagoId = p.MetodoPagoId,
                            Otros = ObtenerOtrosAdicionales(p),
                            MontoTotal = p.MontoTotal,
                            Estado = p.Estado
                        });
                    }
                }

                // 3) Pintar
                var ar = new CultureInfo("es-AR");
                foreach (var r in filas.OrderByDescending(x => x.FechaPago))
                {
                    string estadoTxt = r.Estado == 1 ? "Confirmado" :
                                       r.Estado == 0 ? "Anulado" : "Solicitud";

                    int idx = dataGridPagos.Rows.Add(
                        r.PagoId,                           // Nro comprobante (o PK)
                        r.Periodo,                          // Periodo
                        r.FechaPago.ToString("dd/MM/yyyy"),
                        r.ImporteBase.ToString("C0", ar),   // Monto de cuota
                        r.Mora.ToString("C0", ar),          // Adicional mora
                        ObtenerNombreMetodoPago(r.MetodoPagoId),
                        r.Otros.ToString("C0", ar),         // Otros adicionales
                        r.MontoTotal.ToString("C0", ar),    // Total pagado
                        estadoTxt,                          // Estado
                        null,                                // ColumnaAnular (imagen en Designer)
                        null                                 // ColumnaConfirmar (imagen en Designer)
                    );

                    dataGridPagos.Rows[idx].Tag = r.PagoId; // para acciones
                }

                // 4) KPIs
                decimal totalRecaudado = filas.Where(x => x.Estado == 1).Sum(x => x.MontoTotal);
                int confirmados = filas.Count(x => x.Estado == 1);
                int anulados = filas.Count(x => x.Estado == 0);

                label11.Text = totalRecaudado.ToString("C0", ar);
                label4.Text = confirmados.ToString();
                label6.Text = anulados.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pagos: " + ex.Message, "Pagos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LimpiarGrillaYKpis();
            }
        }

        // ==== Acciones Confirmar/Anular en la grilla ====
        private void dataGridPagos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = dataGridPagos;
            bool isColAnular = grid.Columns[e.ColumnIndex].Name == "ColumnaAnularPago";
            bool isColConfirmar = grid.Columns[e.ColumnIndex].Name == "ColumnaConfirmar";

            if (!isColAnular && !isColConfirmar) return;

            var row = grid.Rows[e.RowIndex];
            if (row.Tag == null || !long.TryParse(row.Tag.ToString(), out long pagoId))
            {
                MessageBox.Show("No se pudo identificar el pago seleccionado.", "Pagos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (isColConfirmar)
                {
                    var ok = _cnPago.ConfirmarPago(pagoId, RolUsuarioActual);
                    if (ok)
                    {
                        MessageBox.Show("Pago confirmado correctamente.", "Pagos",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarPagosDeContratoSeleccionado();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo confirmar el pago.", "Pagos",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (isColAnular)
                {
                    var ok = _cnPago.AnularPago(pagoId, RolUsuarioActual);
                    if (ok)
                    {
                        MessageBox.Show("Pago anulado correctamente.", "Pagos",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarPagosDeContratoSeleccionado();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo anular el pago.", "Pagos",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permisos insuficientes",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Acción no válida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al ejecutar la acción: " + ex.Message, "Pagos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==== Helpers ====
        private void LimpiarGrillaYKpis()
        {
            dataGridPagos.Rows.Clear();
            label11.Text = "$0";
            label4.Text = "0";
            label6.Text = "0";
        }

        private static string ObtenerNombreMetodoPago(int metodoPagoId)
        {
            switch (metodoPagoId)
            {
                case 1: return "Efectivo";
                case 2: return "Transferencia";
                case 3: return "Tarjeta";
                default: return $"MP #{metodoPagoId}";
            }
        }

        // Algunos modelos de Pago no traen “otros” como propiedad; si no existe, queda 0
        private static decimal ObtenerOtrosAdicionales(Pago p)
        {
            // Si tu entidad Pago tiene p.OtrosAdicionalesTotal (o similar), devolvelo aquí.
            return 0m;
        }

        private void CentrarFormHijoSiHubiera()
        {
            // Si más adelante embebés formularios hijos en un panel, centrarlos aquí.
        }

        // DTO local para pintar la grilla
        private sealed class PagoRow
        {
            public long PagoId { get; set; }
            public string Periodo { get; set; }
            public DateTime FechaPago { get; set; }
            public decimal ImporteBase { get; set; }
            public decimal Mora { get; set; }
            public int MetodoPagoId { get; set; }
            public decimal Otros { get; set; }
            public decimal MontoTotal { get; set; }
            public int Estado { get; set; }
        }

        // Item para CB_Propietario (muestra contratos del inquilino)
        private sealed class ContratoItem
        {
            public int Id { get; }
            public string Display { get; }
            public ContratoGridRow Row { get; }

            public ContratoItem(ContratoGridRow row)
            {
                Row = row;
                Id = row.Id;                       // <-- usar tus propiedades reales
                Display = $"Contrato #{row.Id} – {row.Direccion}";
            }

            public override string ToString() => Display;
        }

        private void BRegistrarPago_Click(object sender, EventArgs e)
        {
            // Abre el formulario RegistrarPago
            var form = new RegistrarPago();

            // Pasamos datos de contexto (usuario y rol actual)
            form.UsuarioIdActual = 1;          // reemplazá por el id real si lo tenés
            form.RolUsuarioActual = RolUsuarioActual;

            form.ShowDialog();

            // Actualizar la vista de pagos al cerrar
            CargarPagosDeContratoSeleccionado();
        }

    }

}


