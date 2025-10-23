using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private Usuario usuarioLogueado;

        public Usuarios(Usuario oUsuario)
        {
            InitializeComponent();
            this.Resize += Usuarios_Resize;
            this.usuarioLogueado = oUsuario;
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() => IsOpen(agregarUsuariosForm) || IsOpen(editarUsuarioForm);

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
            comboFiltroUsuarios.SelectedIndex = 0;
            comboFiltroEstado.SelectedIndex = 0;
            CargarGrilla();
        }

        private void comboFiltroUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }
        
        private void comboFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void CargarGrilla()
        {
            string seleccionRol = comboFiltroUsuarios.SelectedItem?.ToString() ?? "Todos";
            RolUsuarioFiltro filtroRol;
            switch (seleccionRol)
            {
                case "Administradores":
                    filtroRol = RolUsuarioFiltro.Administradores;
                    break;
                case "Gerentes":
                    filtroRol = RolUsuarioFiltro.Gerentes;
                    break;
                case "Operadores":
                    filtroRol = RolUsuarioFiltro.Operadores;
                    break;
                default:
                    filtroRol = RolUsuarioFiltro.Todos;
                    break;
            }

            string seleccionEstado = comboFiltroEstado.SelectedItem?.ToString() ?? "Todos";
            EstadoFiltro filtroEstado;
            switch (seleccionEstado)
            {
                case "Activos":
                    filtroEstado = EstadoFiltro.Activos;
                    break;
                case "Inactivos":
                    filtroEstado = EstadoFiltro.Inactivos;
                    break;
                default:
                    filtroEstado = EstadoFiltro.Todos;
                    break;
            }

            try
            {
                var negocio = new CN_Usuario();
                _usuarios = negocio.Listar(filtroRol, filtroEstado);

                dataGridUsuarios.Rows.Clear();
                dataGridUsuarios.AutoGenerateColumns = false;

                foreach (var u in _usuarios)
                {
                    dataGridUsuarios.Rows.Add(new object[]
                    {
                        u.Dni,
                        u.oPersona?.Nombre,
                        u.oPersona?.Apellido,
                        u.oPersona?.Direccion,
                        u.oPersona?.Telefono,
                        u.oPersona?.CorreoElectronico,
                        u.oRolUsuario?.Nombre,
                        u.Estado == 1 ? "Activo" : "Inactivo",
                        null,
                        null
                    });
                }
                ActualizarIndicadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cargar los usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarIndicadores()
        {
            var negocio = new CN_Usuario();
            var estadisticas = negocio.ObtenerEstadisticas();

            label11.Text = estadisticas.Total.ToString();
            label8.Text = estadisticas.Admins.ToString();
            label6.Text = estadisticas.Gerentes.ToString();
            label14.Text = estadisticas.Operadores.ToString();
            label17.Text = estadisticas.Activos.ToString();
            label19.Text = estadisticas.Inactivos.ToString();
        }

        private void BAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarUsuariosForm = new AgregarUsuario();
            agregarUsuariosForm.TopLevel = false;
            agregarUsuariosForm.FormBorderStyle = FormBorderStyle.None;
            ContenedorUsuarios.Controls.Add(agregarUsuariosForm);

            agregarUsuariosForm.FormClosed += (_, __) =>
            {
                ContenedorUsuarios.Controls.Remove(agregarUsuariosForm);
                agregarUsuariosForm.Dispose();
                agregarUsuariosForm = null;
                CargarGrilla();
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
                var dniEditar = dataGridUsuarios.Rows[e.RowIndex].Cells["UsuarioColumna"].Value?.ToString();
                var uSel = _usuarios.FirstOrDefault(u => u.Dni == dniEditar);

                if (uSel == null)
                {
                    MessageBox.Show("No se encontró el usuario para editar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                editarUsuarioForm = new EditarUsuario(uSel, this.usuarioLogueado);
                editarUsuarioForm.TopLevel = false;
                editarUsuarioForm.FormBorderStyle = FormBorderStyle.None;
                ContenedorUsuarios.Controls.Add(editarUsuarioForm);

                editarUsuarioForm.FormClosed += (_, __) =>
                {
                    ContenedorUsuarios.Controls.Remove(editarUsuarioForm);
                    editarUsuarioForm.Dispose();
                    editarUsuarioForm = null;
                    CargarGrilla();
                };

                editarUsuarioForm.Show();
                editarUsuarioForm.BringToFront();
                editarUsuarioForm.Focus();
                CentrarFormAbierto();
            }

            if (colName == "ColumnaAcciones")
            {
                string dni = dataGridUsuarios.Rows[e.RowIndex].Cells["UsuarioColumna"].Value.ToString();

                if (dni == this.usuarioLogueado.Dni)
                {
                    MessageBox.Show("No puede modificar su propio estado desde esta pantalla.", "Acción no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                try
                {
                    var usuarioSeleccionado = _usuarios.FirstOrDefault(u => u.Dni == dni);
                    if (usuarioSeleccionado == null) return;
                    
                    int estadoActual = usuarioSeleccionado.Estado;

                    if (estadoActual == 1)
                    {
                        DialogResult resultado = MessageBox.Show("¿Está seguro de que desea desactivar a este usuario?", "Confirmar Baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                        {
                            if (new CN_Usuario().CambiarEstado(dni, 0))
                            {
                                CargarGrilla();
                            }
                        }
                    }
                    else
                    {
                        DialogResult resultado = MessageBox.Show("¿Desea reactivar este usuario?", "Confirmar Reactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                        {
                            if (new CN_Usuario().CambiarEstado(dni, 1))
                            {
                                CargarGrilla();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _usuarios.Count)
                return;

            Usuario usuarioActual = _usuarios[e.RowIndex];
            int estado = usuarioActual.Estado;
            string columnName = dataGridUsuarios.Columns[e.ColumnIndex].Name;

            if (columnName == "ColumnaEditar")
            {
                e.Value = Properties.Resources.edit;
                dataGridUsuarios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Editar usuario";
            }

            if (columnName == "ColumnaAcciones")
            {
                if (estado == 1)
                {
                    e.Value = Properties.Resources.delete;
                    dataGridUsuarios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Desactivar usuario";
                }
                else
                {
                    e.Value = Properties.Resources.reactive;
                    dataGridUsuarios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Reactivar usuario";
                }
            }

            if (estado == 0)
            {
                e.CellStyle.BackColor = Color.DarkOrange;
                e.CellStyle.ForeColor = Color.White;
            }
            else
            {
                e.CellStyle.BackColor = Color.FromArgb(15, 30, 45);
                e.CellStyle.ForeColor = Color.White;
            }
        }
    }
}