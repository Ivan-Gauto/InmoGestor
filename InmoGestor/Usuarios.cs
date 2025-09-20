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
    public partial class Usuarios : Form
    {
        private AgregarUsuario agregarUsuariosForm;
        private EditarUsuario editarUsuarioForm;

        public Usuarios()
        {
            InitializeComponent();
            this.Resize += Usuarios_Resize;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;

        private bool HayFormAbierto() =>
            IsOpen(agregarUsuariosForm) || IsOpen(editarUsuarioForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarUsuariosForm)) { agregarUsuariosForm.BringToFront(); agregarUsuariosForm.Focus(); return; }
            if (IsOpen(editarUsuarioForm)) { editarUsuarioForm.BringToFront(); editarUsuarioForm.Focus(); return; }

        }

        private void Usuarios_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarUsuariosForm) ? (Form)agregarUsuariosForm
                  : IsOpen(editarUsuarioForm) ? (Form)editarUsuarioForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorUsuarios.Width - f.Width) / 2,
                (ContenedorUsuarios.Height - f.Height) / 2
            );
        }


        private void Usuarios_Load(object sender, EventArgs e)
        {
            dataGridUsuarios.Rows.Clear();

            dataGridUsuarios.Rows.Add("11111111", "Av. Siempre Viva 742", "Juan", "Pérez", "123456789", "juan@mail.com", "Admin", "Activo");
            dataGridUsuarios.Rows.Add("22222222", "Calle Falsa 123", "Ana", "Gómez", "987654321", "ana@mail.com", "Usuario", "Inactivo");
            dataGridUsuarios.Rows.Add("33333333", "San Martín 555", "Carlos", "López", "112233445", "carlos@mail.com", "Operador", "Activo");
            dataGridUsuarios.Rows.Add("44444444", "Belgrano 890", "María", "Fernández", "556677889", "maria@mail.com", "Usuario", "Activo");
            dataGridUsuarios.Rows.Add("55555555", "Mitre 321", "Luis", "Ramírez", "667788990", "luis@mail.com", "Admin", "Inactivo");

        }

        private void BAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarUsuariosForm = new AgregarUsuario
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorUsuarios.Controls.Add(agregarUsuariosForm);

            agregarUsuariosForm.FormClosed += (_, __) =>
            {
                ContenedorUsuarios.Controls.Remove(agregarUsuariosForm);
                agregarUsuariosForm.Dispose();
                agregarUsuariosForm = null;
            };

            agregarUsuariosForm.Show();
            agregarUsuariosForm.BringToFront();
            agregarUsuariosForm.Focus();
            CentrarFormAbierto();
        }


        private void dataGridUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dataGridUsuarios.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar")
            {
                editarUsuarioForm = new EditarUsuario
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorUsuarios.Controls.Add(editarUsuarioForm);

                editarUsuarioForm.FormClosed += (_, __) =>
                {
                    ContenedorUsuarios.Controls.Remove(editarUsuarioForm);
                    editarUsuarioForm.Dispose();
                    editarUsuarioForm = null;
                };

                editarUsuarioForm.Show();
                editarUsuarioForm.BringToFront();
                editarUsuarioForm.Focus();
                CentrarFormAbierto();
            }
            else if (colName == "ColumnaEliminar")
            {
                var result = MessageBox.Show("¿Está seguro de que desea eliminar este usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dataGridUsuarios.Rows.RemoveAt(e.RowIndex);
                }
            }

        }
    }
}
