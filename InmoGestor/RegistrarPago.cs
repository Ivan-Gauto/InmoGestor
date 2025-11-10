using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class RegistrarPago : Form
    {
        // ==== Servicios de Negocio ====
        private readonly CN_Contrato _cnContrato = new CN_Contrato();
        private readonly CN_Cuota _cnCuota = new CN_Cuota();
        private readonly CN_Pago _cnPago = new CN_Pago();

        // ==== Estado de la pantalla ====
        private List<ContratoGridRow> _contratosVigentes = new List<ContratoGridRow>();
        private Cuota _cuotaSel = null;

        // Configurables desde login/sesión
        public int UsuarioIdActual { get; set; } = 1;
        public int RolUsuarioActual { get; set; } = Roles.Operador; // Operador puede confirmar al registrar

        public RegistrarPago()
        {
            InitializeComponent();
        }

        // ======== LOAD (usa el nombre del Designer) ========
        private void RegistrarPago_Load_1(object sender, EventArgs e)
        {
            // Wire de eventos (si no lo hiciste en el diseñador)
            CBInquilinos.SelectedIndexChanged -= CBInquilinos_SelectedIndexChanged;
            CBContratos.SelectedIndexChanged -= CBContratos_SelectedIndexChanged;
            TTadicionales.TextChanged -= TTadicionales_TextChanged;

            CBInquilinos.SelectedIndexChanged += CBInquilinos_SelectedIndexChanged;
            CBContratos.SelectedIndexChanged += CBContratos_SelectedIndexChanged;
            TTadicionales.TextChanged += TTadicionales_TextChanged;

            button_SolicitarA.Click += button_SolicitarA_Click;
            button_confirmarPago.Click += button_confirmarPago_Click;

            CargarContratosVigentes();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ======== Carga de combos ========
        private void CargarContratosVigentes()
        {
            try
            {
                _contratosVigentes = _cnContrato.ListarParaGrid(1) ?? new List<ContratoGridRow>(); // 1 = Vigentes

                // Poblar CBInquilinos con nombres únicos desde los contratos
                var inquilinos = _contratosVigentes
                    .Select(c => c.Inquilino == null ? null : c.Inquilino.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                    .OrderBy(s => s)
                    .ToList();

                CBInquilinos.DataSource = inquilinos;
                if (inquilinos.Count > 0)
                    CBInquilinos.SelectedIndex = 0;
                else
                {
                    CBInquilinos.DataSource = null;
                    CBContratos.DataSource = null;
                    ResetDetalle();
                    HabilitarAcciones(false);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar contratos: " + ex.Message, "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CBInquilinos.DataSource = null;
                CBContratos.DataSource = null;
                ResetDetalle();
                HabilitarAcciones(false);
            }
        }

        private void CBInquilinos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var inq = CBInquilinos.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(inq))
                {
                    CBContratos.DataSource = null;
                    ResetDetalle();
                    HabilitarAcciones(false);
                    return;
                }

                var delInquilino = _contratosVigentes
                    .Where(c => string.Equals(c.Inquilino == null ? null : c.Inquilino.Trim(),
                                              inq == null ? null : inq.Trim(),
                                              StringComparison.InvariantCultureIgnoreCase))
                    .Select(c => new ContratoItem(c))
                    .ToList();

                CBContratos.DisplayMember = nameof(ContratoItem.Display);
                CBContratos.ValueMember = nameof(ContratoItem.Id);
                CBContratos.DataSource = delInquilino;

                if (delInquilino.Count > 0)
                    CBContratos.SelectedIndex = 0;
                else
                {
                    ResetDetalle();
                    HabilitarAcciones(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar contratos: " + ex.Message, "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CBContratos_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarPrimeraCuotaPendiente();
        }

        // ======== Lógica de cuotas & cálculo ========
        private void CargarPrimeraCuotaPendiente()
        {
            ResetDetalle();

            var item = CBContratos.SelectedItem as ContratoItem;
            if (item == null)
            {
                HabilitarAcciones(false);
                return;
            }

            // Mostrar datos del encabezado
            label_Inquilino.Text = item.Row.Inquilino ?? "-";
            label_Propietario.Text = "-"; // no lo tenemos en este grid

            try
            {
                var cuotas = _cnCuota.ListarPorContrato(item.Id) ?? new List<Cuota>();
                _cuotaSel = cuotas.FirstOrDefault(q => q.Estado == 0); // 0 = Pendiente

                if (_cuotaSel == null)
                {
                    label_periodo.Text = "-";
                    label_NroCuota.Text = "-";
                    label_vencimiento.Text = "-";
                    label_importeBase.Text = "$0";
                    label_AdicionalMora.Text = "$0";
                    label_OtrosAdicionales.Text = "$0";
                    label_TotalPagar.Text = "$0";
                    SetEstadoVisual("Sin deuda", System.Drawing.Color.Gray);
                    HabilitarAcciones(false);
                    return;
                }

                // Llenar datos cuota
                label_NroCuota.Text = _cuotaSel.NroCuota.ToString();
                label_periodo.Text = PeriodoLargo(_cuotaSel.Periodo);
                label_vencimiento.Text = _cuotaSel.FechaVenc.ToString("dd/MM/yyyy");

                var ar = new CultureInfo("es-AR");
                var importe = _cuotaSel.ImporteBase;
                var mora = CalcularMoraRegla15(_cuotaSel);

                // Otros desde textbox
                var otros = ParseOtros(TTadicionales.Text);

                label_importeBase.Text = importe.ToString("C0", ar);
                label_AdicionalMora.Text = mora.ToString("C0", ar);
                label_OtrosAdicionales.Text = otros.ToString("C0", ar);

                var total = importe + mora + otros;
                label_TotalPagar.Text = total.ToString("C0", ar);

                // Estado visual
                var inicioMoraAprox = _cuotaSel.FechaVenc.AddDays(-15);
                if (DateTime.Today <= inicioMoraAprox)
                    SetEstadoVisual("Pendiente", System.Drawing.Color.Yellow);
                else
                    SetEstadoVisual("En mora", System.Drawing.Color.Red);

                HabilitarAcciones(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cuota: " + ex.Message, "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                HabilitarAcciones(false);
            }
        }

        private void TTadicionales_TextChanged(object sender, EventArgs e)
        {
            if (_cuotaSel == null) return;

            var ar = new CultureInfo("es-AR");
            var importe = _cuotaSel.ImporteBase;
            var mora = CalcularMoraRegla15(_cuotaSel);
            var otros = ParseOtros(TTadicionales.Text);

            label_OtrosAdicionales.Text = otros.ToString("C0", ar);
            label_TotalPagar.Text = (importe + mora + otros).ToString("C0", ar);
        }

        private static decimal CalcularMoraRegla15(Cuota c)
        {
            var inicioAprox = c.FechaVenc.AddDays(-15);
            if (DateTime.Today <= inicioAprox) return 0m;

            var tasaMensual = c.TasaMoraMensualAplicada; // %
            var mora = c.ImporteBase * (tasaMensual / 100m);
            return Math.Round(mora, 2);
        }

        private static decimal ParseOtros(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt)) return 0m;
            decimal v;
            if (decimal.TryParse(txt.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out v))
                return v < 0 ? 0m : Math.Round(v, 2);
            return 0m;
        }

        private static string PeriodoLargo(string periodoAAAAMM)
        {
            if (string.IsNullOrWhiteSpace(periodoAAAAMM) || periodoAAAAMM.Length != 6) return periodoAAAAMM ?? "-";

            int anio;
            int mes;
            if (!int.TryParse(periodoAAAAMM.Substring(0, 4), out anio)) return periodoAAAAMM;
            if (!int.TryParse(periodoAAAAMM.Substring(4, 2), out mes)) return periodoAAAAMM;

            var nombreMes = new DateTime(anio, mes, 1).ToString("MMMM", new CultureInfo("es-AR"));
            if (string.IsNullOrEmpty(nombreMes)) return periodoAAAAMM;

            return char.ToUpper(nombreMes[0]) + (nombreMes.Length > 1 ? nombreMes.Substring(1) : "") + " de " + anio;
        }

        private void SetEstadoVisual(string texto, System.Drawing.Color backColor)
        {
            LEstado.Text = texto;
            LEstado.BackColor = backColor;
        }

        private void ResetDetalle()
        {
            _cuotaSel = null;
            label_Inquilino.Text = "-";
            label_Propietario.Text = "-";
            label_periodo.Text = "-";
            label_NroCuota.Text = "-";
            label_vencimiento.Text = "-";
            label_importeBase.Text = "$0";
            label_AdicionalMora.Text = "$0";
            label_OtrosAdicionales.Text = "$0";
            label_TotalPagar.Text = "$0";
            SetEstadoVisual("Pendiente", System.Drawing.Color.SeaGreen);
        }

        private void HabilitarAcciones(bool on)
        {
            button_SolicitarA.Enabled = on && _cuotaSel != null;
            button_confirmarPago.Enabled = on && _cuotaSel != null;
        }

        // ======== Acciones ========

        private void button_SolicitarA_Click(object sender, EventArgs e)
        {
            if (_cuotaSel == null)
            {
                MessageBox.Show("No hay cuota pendiente para registrar.", "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var pago = ConstruirPago(2); // Solicitud
                var id = _cnPago.RegistrarPago(pago, RolUsuarioActual);

                if (id > 0)
                {
                    MessageBox.Show("Pago registrado en 'Solicitud de aprobación'.", "Registrar pago",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el pago.", "Registrar pago",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al solicitar aprobación: " + ex.Message, "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_confirmarPago_Click(object sender, EventArgs e)
        {
            if (_cuotaSel == null)
            {
                MessageBox.Show("No hay cuota pendiente para registrar.", "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var pago = ConstruirPago(1); // Confirmado
                var id = _cnPago.RegistrarPago(pago, RolUsuarioActual);

                if (id > 0)
                {
                    MessageBox.Show("Pago confirmado correctamente.", "Registrar pago",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("No se pudo confirmar el pago.", "Registrar pago",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al confirmar pago: " + ex.Message, "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Pago ConstruirPago(int estado)
        {
            var importe = _cuotaSel.ImporteBase;
            var mora = CalcularMoraRegla15(_cuotaSel);
            var otros = ParseOtros(TTadicionales.Text);
            var total = importe + mora + otros;

            return new Pago
            {
                FechaPago = DateTime.Today,
                MontoTotal = total,
                Periodo = _cuotaSel.Periodo,
                MetodoPagoId = 1,    // si luego agregás combo de métodos, reemplazá aquí
                Estado = estado,     // 1=Confirmado, 2=Solicitud
                CuotaId = _cuotaSel.CuotaId,
                MoraCobrada = mora,
                UsuarioCreador = 3
            };
        }

        // ======== Helpers internos ========
        private sealed class ContratoItem
        {
            public int Id { get { return Row.Id; } }
            public string Display { get; private set; }
            public ContratoGridRow Row { get; private set; }

            public ContratoItem(ContratoGridRow row)
            {
                Row = row;
                Display = "Contrato #" + row.Id + " – " + row.Direccion;
            }

            public override string ToString()
            {
                return Display;
            }
        }
    }
}
