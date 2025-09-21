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
    public partial class Inmuebles : Form
    {
        private EditarInmueble editarInmuebleForm;
        private AgregarInmueble agregarInmuebleForm;

        public Inmuebles()
        {
            InitializeComponent();
            this.Resize += Inmueble_Resize;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() =>
            IsOpen(agregarInmuebleForm) || IsOpen(editarInmuebleForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarInmuebleForm)) { agregarInmuebleForm.BringToFront(); agregarInmuebleForm.Focus(); return; }
            if (IsOpen(editarInmuebleForm)) { editarInmuebleForm.BringToFront(); editarInmuebleForm.Focus(); return; }

        }

        private void Inmueble_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarInmuebleForm) ? (Form)agregarInmuebleForm
                  : IsOpen(editarInmuebleForm) ? (Form)editarInmuebleForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorInmuebles.Width - f.Width) / 2,
                (ContenedorInmuebles.Height - f.Height) / 2
            );
        }

        private void BAgregarInmueble_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarInmuebleForm = new AgregarInmueble
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorInmuebles.Controls.Add(agregarInmuebleForm);

            agregarInmuebleForm.FormClosed += (_, __) =>
            {
                ContenedorInmuebles.Controls.Remove(agregarInmuebleForm);
                agregarInmuebleForm.Dispose();
                agregarInmuebleForm = null;
            };

            agregarInmuebleForm.Show();
            agregarInmuebleForm.BringToFront();
            agregarInmuebleForm.Focus();
            CentrarFormAbierto();
        }

        private void dataGridInmuebles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dataGridInmuebles.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar")
            {
                editarInmuebleForm = new EditarInmueble
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorInmuebles.Controls.Add(editarInmuebleForm);

                editarInmuebleForm.FormClosed += (_, __) =>
                {
                    ContenedorInmuebles.Controls.Remove(editarInmuebleForm);
                    editarInmuebleForm.Dispose();
                    editarInmuebleForm = null;
                };

                editarInmuebleForm.Show();
                editarInmuebleForm.BringToFront();
                editarInmuebleForm.Focus();
                CentrarFormAbierto();
            }
            else if (colName == "ColumnaEliminar")
            {
                var result = MessageBox.Show("¿Está seguro de que desea eliminar este inquilino?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dataGridInmuebles.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void Inmuebles_Load(object sender, EventArgs e)
        {
            dataGridInmuebles.Rows.Add("1", "Av. Corrientes 1200", "Depto 2 ambientes", "Juan Pérez", "Departamento", "Ocupado");
            dataGridInmuebles.Rows.Add("2", "San Martín 450", "Casa de 3 dormitorios", "María González", "Casa", "Disponible");
            dataGridInmuebles.Rows.Add("3", "Belgrano 789", "Local comercial", "Carlos López", "Local", "En reparación");
            dataGridInmuebles.Rows.Add("4", "Rivadavia 234", "Depto monoambiente", "Ana Fernández", "Departamento", "Ocupado");
            dataGridInmuebles.Rows.Add("5", "Junín 1020", "Casa con patio", "Diego Martínez", "Casa", "Disponible");
            dataGridInmuebles.Rows.Add("6", "Rioja 550", "Oficina 1er piso", "Laura Ramírez", "Oficina", "Ocupado");
        }


    }
}
