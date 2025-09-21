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
    public partial class Propietarios : Form
    {
        private EditarPropietario editarPropietarioForm;
        private AgregarPropietario agregarPropietarioForm;

        public Propietarios()
        {
            InitializeComponent();
            this.Resize += Propietarios_Resize;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() =>
            IsOpen(agregarPropietarioForm) || IsOpen(editarPropietarioForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarPropietarioForm)) { agregarPropietarioForm.BringToFront(); agregarPropietarioForm.Focus(); return; }
            if (IsOpen(editarPropietarioForm)) { editarPropietarioForm.BringToFront(); editarPropietarioForm.Focus(); return; }

        }

        private void Propietarios_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarPropietarioForm) ? (Form)agregarPropietarioForm
                  : IsOpen(editarPropietarioForm) ? (Form)editarPropietarioForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorPropietarios.Width - f.Width) / 2,
                (ContenedorPropietarios.Height - f.Height) / 2
            );
        }

        private void BAgregarPropietario_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarPropietarioForm = new AgregarPropietario
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorPropietarios.Controls.Add(agregarPropietarioForm);

            agregarPropietarioForm.FormClosed += (_, __) =>
            {
                ContenedorPropietarios.Controls.Remove(agregarPropietarioForm);
                agregarPropietarioForm.Dispose();
                agregarPropietarioForm = null;
            };

            agregarPropietarioForm.Show();
            agregarPropietarioForm.BringToFront();
            agregarPropietarioForm.Focus();
            CentrarFormAbierto();
        }

        private void Propietarios_Load(object sender, EventArgs e)
        {
            dataGridPropietarios.Rows.Add("30111222", "Av. Corrientes 1234", "Juan", "Pérez", "3794123456", "juan.perez@mail.com", "$120000", "2", "Activo");
            dataGridPropietarios.Rows.Add("28456789", "San Martín 450", "María", "González", "3794987654", "maria.gonzalez@mail.com", "$85000", "1", "Activo");
            dataGridPropietarios.Rows.Add("33123456", "Belgrano 789", "Carlos", "López", "3794678901", "carlos.lopez@mail.com", "$60000", "0", "Inactivo");
            dataGridPropietarios.Rows.Add("29876543", "Rivadavia 234", "Ana", "Fernández", "3794456123", "ana.fernandez@mail.com", "$150000", "3", "Activo");
            dataGridPropietarios.Rows.Add("31567890", "Junín 1020", "Diego", "Martínez", "3794567890", "diego.martinez@mail.com", "$40000", "1", "Activo");
            dataGridPropietarios.Rows.Add("27543210", "Rioja 550", "Laura", "Ramírez", "3794234567", "laura.ramirez@mail.com", "$0", "0", "Inactivo");
        }

        private void dataGridPropietarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dataGridPropietarios.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar")
            {
                editarPropietarioForm = new EditarPropietario
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorPropietarios.Controls.Add(editarPropietarioForm);

                editarPropietarioForm.FormClosed += (_, __) =>
                {
                    ContenedorPropietarios.Controls.Remove(editarPropietarioForm);
                    editarPropietarioForm.Dispose();
                    editarPropietarioForm = null;
                };

                editarPropietarioForm.Show();
                editarPropietarioForm.BringToFront();
                editarPropietarioForm.Focus();
                CentrarFormAbierto();
            }
            else if (colName == "ColumnaEliminar")
            {
                var result = MessageBox.Show("¿Está seguro de que desea eliminar este propietario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dataGridPropietarios.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
    }
}
