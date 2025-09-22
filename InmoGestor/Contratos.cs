using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InmoGestor
{
    public partial class Contratos : Form
    {
        private EditarContrato editarContratoForm;
        private AgregarContrato agregarContratoForm;

        public Contratos()
        {
            InitializeComponent();
            this.Resize += Contratos_Resize;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() =>
            IsOpen(agregarContratoForm) || IsOpen(editarContratoForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarContratoForm)) { agregarContratoForm.BringToFront(); agregarContratoForm.Focus(); return; }
            if (IsOpen(editarContratoForm)) { editarContratoForm.BringToFront(); editarContratoForm.Focus(); return; }

        }

        private void Contratos_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarContratoForm) ? (Form)agregarContratoForm
                  : IsOpen(editarContratoForm) ? (Form)editarContratoForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorContratos.Width - f.Width) / 2,
                (ContenedorContratos.Height - f.Height) / 2
            );
        }

        private void BAgregarContrato_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarContratoForm = new AgregarContrato
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorContratos.Controls.Add(agregarContratoForm);

            agregarContratoForm.FormClosed += (_, __) =>
            {
                ContenedorContratos.Controls.Remove(agregarContratoForm);
                agregarContratoForm.Dispose();
                agregarContratoForm = null;
            };

            agregarContratoForm.Show();
            agregarContratoForm.BringToFront();
            agregarContratoForm.Focus();
            CentrarFormAbierto();
        }



        private void dataGridContratos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dataGridContratos.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar")
            {
                editarContratoForm = new EditarContrato
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorContratos.Controls.Add(editarContratoForm);

                editarContratoForm.FormClosed += (_, __) =>
                {
                    ContenedorContratos.Controls.Remove(editarContratoForm);
                    editarContratoForm.Dispose();
                    editarContratoForm = null;
                };

                editarContratoForm.Show();
                editarContratoForm.BringToFront();
                editarContratoForm.Focus();
                CentrarFormAbierto();
            }
            else if (colName == "ColumnaEliminar")
            {
                var result = MessageBox.Show("¿Está seguro de que desea eliminar este inquilino?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dataGridContratos.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void Contratos_Load(object sender, EventArgs e)
        {
            dataGridContratos.Rows.Add("1", "Juan Pérez", "Av. Corrientes 1200", "Depto 2B", "120000", "12", "01/01/2025", "01/01/2026");
            dataGridContratos.Rows.Add("2", "María López", "San Martín 850", "Casa 3", "95000", "10", "15/02/2025", "15/12/2025");
            dataGridContratos.Rows.Add("3", "Carlos Gómez", "Av. Rivadavia 3000", "Local A", "180000", "24", "01/03/2025", "01/03/2027");
            dataGridContratos.Rows.Add("4", "Laura Fernández", "Belgrano 455", "Depto 1A", "110000", "18", "10/04/2025", "10/10/2026");
            dataGridContratos.Rows.Add("5", "Diego Martínez", "Mitre 2200", "PH Fondo", "135000", "12", "01/05/2025", "01/05/2026");
        }

    }
}
