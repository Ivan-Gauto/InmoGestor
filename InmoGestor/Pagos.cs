using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private List<Pagos> _pagos = new List<Pagos>();
        public Pagos()
        {
            InitializeComponent();
            this.Resize += Pagos_Resize;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() =>
            IsOpen(registrarPagoForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(registrarPagoForm)) { registrarPagoForm.BringToFront(); registrarPagoForm.Focus(); return; }
        }

        private void Pagos_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(registrarPagoForm) ? (Form)registrarPagoForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorPagos.Width - f.Width) / 2,
                (ContenedorPagos.Height - f.Height) / 2
            );
        }

        private void Usuarios_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void Pagos_Load(object sender, EventArgs e)
        {
            dataGridPagos.Rows.Clear();

            // ID, NroComp(00001), Periodo, FechaPago, Cuota, Mora, Metodo, Otros, Total, Estado, Botón
            dataGridPagos.Rows.Add("1", 1.ToString("D5"), "2025-10", "2025-09-21",
                                   250000m, 0m, "Transferencia", 0m, 250000m, "Activo");

            dataGridPagos.Rows.Add("2", 2.ToString("D5"), "2025-11", "2025-10-25",
                                   260000m, 5000m, "Efectivo", 2500m, 260000m + 5000m + 2500m, "Activo");

            dataGridPagos.Rows.Add("3", 3.ToString("D5"), "2025-12", "2025-12-02",
                                   270000m, 0m, "Tarjeta", 1000m, 270000m + 0m + 1000m, "Anulado");
        }

        private void BRegistrarPago_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            registrarPagoForm = new RegistrarPago
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorPagos.Controls.Add(registrarPagoForm);

            registrarPagoForm.FormClosed += (_, __) =>
            {
                ContenedorPagos.Controls.Remove(registrarPagoForm);
                registrarPagoForm.Dispose();
                registrarPagoForm = null;
            };

            registrarPagoForm.Show();
            registrarPagoForm.BringToFront();
            registrarPagoForm.Focus();
            CentrarFormAbierto();
        }
    }
}
