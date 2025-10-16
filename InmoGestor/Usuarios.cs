using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class Usuarios : Form
    {
        private AgregarUsuario agregarUsuariosForm;
        private EditarUsuario editarUsuarioForm;

        private List<Usuario> _usuarios = new List<Usuario>();

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
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            dataGridUsuarios.Rows.Clear();
            dataGridUsuarios.AutoGenerateColumns = false;

            _usuarios = new CN_Usuario().Listar() ?? new List<Usuario>();

            foreach (var u in _usuarios)
            {
                dataGridUsuarios.Rows.Add(
                    u.Dni,
                    u.oPersona?.Nombre ?? "",
                    u.oPersona?.Apellido ?? "",
                    u.oPersona?.Direccion ?? "",
                    u.oPersona?.Telefono ?? "",
                    u.oPersona?.CorreoElectronico ?? "",
                    u.oRolUsuario?.Nombre ?? "",
                    u.Estado ? "Activo" : "Inactivo"
                );
            }

            // contadores
            label11.Text = _usuarios.Count.ToString();
            label8.Text = _usuarios.Count(x => x.RolUsuarioId == 1).ToString(); // Admin
            label6.Text = _usuarios.Count(x => x.RolUsuarioId == 4).ToString(); // Gerente
            label14.Text = _usuarios.Count(x => x.RolUsuarioId == 2).ToString(); // Operador
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
                CargarUsuarios();
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
                // 1) tomar DNI desde la grilla
                var dni = dataGridUsuarios.Rows[e.RowIndex]
                                          .Cells["UsuarioColumna"] // <-- Asegurate que la columna DNI se llame así
                                          .Value?.ToString();

                // 2) buscar el objeto completo en la lista en memoria
                var uSel = _usuarios.FirstOrDefault(x => x.Dni == dni);
                if (uSel == null)
                {
                    MessageBox.Show("No se encontró el usuario.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (HayFormAbierto()) { FocusFormAbierto(); return; }

                // 3) abrir el editor PASÁNDOLE el usuario
                editarUsuarioForm = new EditarUsuario(uSel)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorUsuarios.Controls.Add(editarUsuarioForm);

                // refrescar al cerrar
                editarUsuarioForm.FormClosed += (_, __) =>
                {
                    ContenedorUsuarios.Controls.Remove(editarUsuarioForm);
                    editarUsuarioForm.Dispose();
                    editarUsuarioForm = null;
                    CargarUsuarios();
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
