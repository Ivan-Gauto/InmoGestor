using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;
using System.Drawing;
using System.Drawing.Printing;


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

            // --- Columna Imprimir Recibo (una sola) ---
            if (!dataGridPagos.Columns.Contains("ColumnaImprimirRecibo"))
            {
                var colImp = new DataGridViewButtonColumn
                {
                    Name = "ColumnaImprimirRecibo",
                    HeaderText = "Recibo",
                    Text = "Imprimir",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                };
                dataGridPagos.Columns.Add(colImp);
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
                            Estado = p.Estado,
                            CuotaId = q.CuotaId // <<< agregado
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

                    // FIX: usar dataGridPagos (no ddataGridPagos) + Tag con toda la info
                    dataGridPagos.Rows[idx].Tag = new PagoRowTag
                    {
                        PagoId = r.PagoId,
                        CuotaId = r.CuotaId,
                        Periodo = r.Periodo,
                        ImporteBase = r.ImporteBase,
                        Mora = r.Mora,
                        Otros = r.Otros,
                        Total = r.MontoTotal,
                        MetodoPagoId = r.MetodoPagoId
                    };
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
            bool isColImprimir = grid.Columns[e.ColumnIndex].Name == "ColumnaImprimirRecibo";

            if (!isColAnular && !isColConfirmar && !isColImprimir) return;

            if (isColImprimir)
            {
                if (grid.Rows[e.RowIndex].Tag is PagoRowTag tag)
                {
                    ImprimirReciboDesdeTag(tag);
                }
                else
                {
                    MessageBox.Show("No se pudo leer los datos del pago para imprimir.", "Pagos",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
                var row = grid.Rows[e.RowIndex];

            // Para confirmar/anular usamos solo el PagoId (antes estaba como string simple).
            long pagoIdDeTag = 0;
            if (row.Tag is PagoRowTag prt)
                pagoIdDeTag = prt.PagoId;

            if ((isColAnular || isColConfirmar) && pagoIdDeTag <= 0)
            {
                MessageBox.Show("No se pudo identificar el pago seleccionado.", "Pagos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (isColConfirmar)
                {
                    var ok = _cnPago.ConfirmarPago(pagoIdDeTag, RolUsuarioActual);
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
                    var ok = _cnPago.AnularPago(pagoIdDeTag, RolUsuarioActual);
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
                else if (isColImprimir)
                {
                    // Acá después llamás a tu lógica de impresión.
                    MessageBox.Show("Impresión de recibo: pronto aquí (usa el Tag con todos los datos).",
                        "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ImprimirReciboDesdeTag(PagoRowTag tag)
        {
            // Tomamos algunos textos del contexto actual
            string inquilino = CB_Inquilino?.SelectedItem as string ?? "-";
            string contrato = CB_Propietario?.SelectedItem?.ToString() ?? "-";

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar recibo en PDF";
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = $"Recibo_{tag.PagoId}_{tag.Periodo}.pdf";

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                // Buscamos la impresora Microsoft Print to PDF
                var printerName = GetMicrosoftPrintToPdfName();
                if (printerName == null)
                {
                    MessageBox.Show("No se encontró la impresora \"Microsoft Print to PDF\" en este equipo.\n" +
                                    "Instalala o seleccioná otra impresora PDF.",
                                    "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = printerName;
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = sfd.FileName;
                printDoc.DocumentName = $"Recibo #{tag.PagoId}";
                printDoc.OriginAtMargins = true;

                // márgenes
                printDoc.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);

                // Para evitar el cuadro de diálogo de impresión
                printDoc.PrintController = new StandardPrintController();

                // Datos que vamos a dibujar
                printDoc.PrintPage += (sender, e) =>
                {
                    RenderRecibo(e, tag, inquilino, contrato);
                    e.HasMorePages = false;
                };

                try
                {
                    printDoc.Print();
                    MessageBox.Show("Recibo generado correctamente.", "Pagos",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo generar el PDF del recibo.\n\n" + ex.Message,
                        "Pagos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string GetMicrosoftPrintToPdfName()
        {
            foreach (string prn in PrinterSettings.InstalledPrinters)
            {
                if (prn != null && prn.IndexOf("Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase) >= 0)
                    return prn;
            }
            return null;
        }

        private void RenderRecibo(PrintPageEventArgs e, PagoRowTag tag, string inquilino, string contrato)
        {
            var g = e.Graphics;

            // Fuentes y pinceles
            using (var fTitulo = new Font("Segoe UI", 16, FontStyle.Bold))
            using (var fSub = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var fText = new Font("Segoe UI", 10))
            using (var pen = new Pen(Color.Black, 1))
            {
                float y = 0;
                float x = 0;
                float ancho = e.MarginBounds.Width;

                // Encabezado
                g.DrawString("RECIBO DE PAGO", fTitulo, Brushes.Black, x, y);
                y += 35;
                g.DrawLine(pen, x, y, x + ancho, y);
                y += 10;

                // Datos principales
                g.DrawString($"N° Comprobante: {tag.PagoId}", fSub, Brushes.Black, x, y); y += 20;
                g.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy}", fText, Brushes.Black, x, y); y += 22;
                g.DrawString($"Inquilino: {inquilino}", fText, Brushes.Black, x, y); y += 22;
                g.DrawString($"Contrato: {contrato}", fText, Brushes.Black, x, y); y += 22;
                g.DrawString($"Periodo: {tag.Periodo}", fText, Brushes.Black, x, y); y += 28;

                // Detalle importe
                g.DrawString("Detalle", fSub, Brushes.Black, x, y);
                y += 18;
                g.DrawLine(pen, x, y, x + ancho, y);
                y += 10;

                var ar = new CultureInfo("es-AR");
                g.DrawString("Importe base:", fText, Brushes.Black, x, y);
                g.DrawString(tag.ImporteBase.ToString("C0", ar), fText, Brushes.Black, x + ancho - 180, y);
                y += 20;

                g.DrawString("Adicional por mora:", fText, Brushes.Black, x, y);
                g.DrawString(tag.Mora.ToString("C0", ar), fText, Brushes.Black, x + ancho - 180, y);
                y += 20;

                g.DrawString("Otros adicionales:", fText, Brushes.Black, x, y);
                g.DrawString(tag.Otros.ToString("C0", ar), fText, Brushes.Black, x + ancho - 180, y);
                y += 20;

                g.DrawLine(pen, x, y, x + ancho, y); y += 8;

                g.DrawString("TOTAL:", fSub, Brushes.Black, x, y);
                g.DrawString(tag.Total.ToString("C0", ar), fSub, Brushes.Black, x + ancho - 180, y);
                y += 28;

                // Método de pago
                g.DrawString("Método de pago:", fSub, Brushes.Black, x, y);
                g.DrawString(ObtenerNombreMetodoPago(tag.MetodoPagoId), fText, Brushes.Black, x + 140, y);
                y += 28;

                // Pie
                g.DrawLine(pen, x, y, x + ancho, y); y += 40;
                g.DrawString("Firma y aclaración:", fText, Brushes.Black, x, y);
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

            // Agregado para poder imprimir
            public int CuotaId { get; set; }
        }

        // Tag que colgamos en cada Row del grid para imprimir/acciones
        private sealed class PagoRowTag
        {
            public long PagoId { get; set; }
            public int CuotaId { get; set; }
            public string Periodo { get; set; }
            public decimal ImporteBase { get; set; }
            public decimal Mora { get; set; }
            public decimal Otros { get; set; }
            public decimal Total { get; set; }
            public int MetodoPagoId { get; set; }
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
