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
    public partial class Inquilinos : Form
    {
        private EditarInmueble editarInquilinoForm;
        private AgregarInquilino agregarInquilinoForm;

        public Inquilinos()
        {
            InitializeComponent();
            this.Resize += Inquilinos_Resize;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() =>
            IsOpen(agregarInquilinoForm) || IsOpen(editarInquilinoForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarInquilinoForm)) { agregarInquilinoForm.BringToFront(); agregarInquilinoForm.Focus(); return; }
            if (IsOpen(editarInquilinoForm)) { editarInquilinoForm.BringToFront(); editarInquilinoForm.Focus(); return; }

        }

        private void Inquilinos_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarInquilinoForm) ? (Form)agregarInquilinoForm
                  : IsOpen(editarInquilinoForm) ? (Form)editarInquilinoForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorInquilinos.Width - f.Width) / 2,
                (ContenedorInquilinos.Height - f.Height) / 2
            );
        }

        private void BAgregarInquilino_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarInquilinoForm = new AgregarInquilino
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorInquilinos.Controls.Add(agregarInquilinoForm);

            agregarInquilinoForm.FormClosed += (_, __) =>
            {
                ContenedorInquilinos.Controls.Remove(agregarInquilinoForm);
                agregarInquilinoForm.Dispose();
                agregarInquilinoForm = null;
            };

            agregarInquilinoForm.Show();
            agregarInquilinoForm.BringToFront();
            agregarInquilinoForm.Focus();
            CentrarFormAbierto();
        }

        private void dataGridInquilinos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dataGridInquilinos.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar")
            {
                editarInquilinoForm = new EditarInmueble
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorInquilinos.Controls.Add(editarInquilinoForm);

                editarInquilinoForm.FormClosed += (_, __) =>
                {
                    ContenedorInquilinos.Controls.Remove(editarInquilinoForm);
                    editarInquilinoForm.Dispose();
                    editarInquilinoForm = null;
                };

                editarInquilinoForm.Show();
                editarInquilinoForm.BringToFront();
                editarInquilinoForm.Focus();
                CentrarFormAbierto();
            }
            else if (colName == "ColumnaEliminar")
            {
                var result = MessageBox.Show("¿Está seguro de que desea eliminar este inquilino?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dataGridInquilinos.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void Inquilinos_Load(object sender, EventArgs e)
        {
            dataGridInquilinos.Rows.Clear();

            dataGridInquilinos.Rows.Add("11111111", "Av. Siempre Viva 742", "Juan", "Pérez", "123456789", "juan@mail.com", "300000", "10/10/10", "activo");
            dataGridInquilinos.Rows.Add("22222222", "Calle Falsa 123", "Ana", "Gómez", "987654321", "ana@mail.com", "3000000", "10/10/10", "activo");
            dataGridInquilinos.Rows.Add("33333333", "San Martín 555", "Carlos", "López", "112233445", "carlos@mail.com", "200000", "10/10/10", "activo");
            dataGridInquilinos.Rows.Add("44444444", "Belgrano 890", "María", "Fernández", "556677889", "maria@mail.com", "250000", "10/10/10", "activo");
            dataGridInquilinos.Rows.Add("55555555", "Mitre 321", "Luis", "Ramírez", "667788990", "luis@mail.com", "200000", "10/10/10", "activo");
        }
    }
}
