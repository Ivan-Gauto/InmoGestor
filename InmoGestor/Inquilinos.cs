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
    public partial class Inquilinos : Form
    {
        private EditarInquilino editarInquilinoForm;
        private AgregarInquilino agregarInquilinoForm;
        private List<PersonaRolCliente> _inquilinos = new List<PersonaRolCliente>();
        private Usuario usuarioLogueado;

        public Inquilinos(Usuario oUsuario)
        {
            InitializeComponent();
            this.Resize += Inquilinos_Resize;
            this.usuarioLogueado = oUsuario;
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

            agregarInquilinoForm = new AgregarInquilino();
            agregarInquilinoForm.TopLevel = false;
            agregarInquilinoForm.FormBorderStyle = FormBorderStyle.None;
            ContenedorInquilinos.Controls.Add(agregarInquilinoForm);

            agregarInquilinoForm.FormClosed += (_, __) =>
            {
                ContenedorInquilinos.Controls.Remove(agregarInquilinoForm);
                agregarInquilinoForm.Dispose();
                agregarInquilinoForm = null;
                CargarInquilinos();
            };

            agregarInquilinoForm.Show();
            agregarInquilinoForm.BringToFront();
            agregarInquilinoForm.Focus();
            CentrarFormAbierto();
        }

        private void dataGridInquilinos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string colName = dataGridInquilinos.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar")
            {
                var dniEditar = dataGridInquilinos.Rows[e.RowIndex].Cells["ColumnaDni"].Value?.ToString();
                var iSel = _inquilinos.FirstOrDefault(x => x.Dni == dniEditar);

                if (iSel == null)
                {
                    MessageBox.Show("No se encontró el inquilino.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                editarInquilinoForm = new EditarInquilino(iSel);
                editarInquilinoForm.TopLevel = false;
                editarInquilinoForm.FormBorderStyle = FormBorderStyle.None;
                ContenedorInquilinos.Controls.Add(editarInquilinoForm);

                editarInquilinoForm.FormClosed += (_, __) =>
                {
                    ContenedorInquilinos.Controls.Remove(editarInquilinoForm);
                    editarInquilinoForm.Dispose();
                    editarInquilinoForm = null;
                    CargarInquilinos();
                };

                editarInquilinoForm.Show();
                editarInquilinoForm.BringToFront();
                editarInquilinoForm.Focus();
                CentrarFormAbierto();
            }

            if (colName == "ColumnaAcciones")
            {
                try
                {
                    string dni = dataGridInquilinos.Rows[e.RowIndex].Cells["ColumnaDni"].Value.ToString();
                    var inquilinoSeleccionado = _inquilinos.FirstOrDefault(i => i.Dni == dni);
                    if (inquilinoSeleccionado == null) return;

                    // ----- CAMBIO 1 -----
                    // Leemos el estado desde la entidad PersonaRolCliente, no desde Persona
                    // Asumo que tu entidad 'PersonaRolCliente' tiene una propiedad 'Estado'
                    int estadoActual = inquilinoSeleccionado.Estado;

                    // ----- CAMBIO 2 -----
                    // Definimos el ID del rol que estamos gestionando en esta grilla
                    // Asumo que tu enum TipoRolCliente tiene un miembro 'Inquilino'
                    int idRolInquilino = (int)TipoRolCliente.Inquilino;


                    if (estadoActual == 1)
                    {
                        DialogResult resultado = MessageBox.Show("¿Está seguro de que desea desactivar a este inquilino?", "Confirmar Baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                        {
                            // Pasamos el IdRolCliente
                            if (new CN_PersonaRolCliente().CambiarEstado(dni, 0, idRolInquilino))
                            {
                                CargarInquilinos();
                            }
                        }
                    }
                    else
                    {
                        DialogResult resultado = MessageBox.Show("¿Desea reactivar este inquilino?", "Confirmar Reactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                        {
                            // Pasamos el IdRolCliente
                            if (new CN_PersonaRolCliente().CambiarEstado(dni, 1, idRolInquilino))
                            {
                                CargarInquilinos();
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

        private void dataGridInquilinos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _inquilinos.Count)
                return;

            PersonaRolCliente inquilinoActual = _inquilinos[e.RowIndex];

            // ----- MODIFICADO -----
            // Leemos el estado del rol (persona_rol_cliente), no de la persona
            int estado = inquilinoActual.Estado;

            string columnName = dataGridInquilinos.Columns[e.ColumnIndex].Name;

            if (columnName == "ColumnaEditar")
            {
                e.Value = Properties.Resources.edit;
                dataGridInquilinos.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Editar inquilino";
            }

            if (columnName == "ColumnaAcciones")
            {
                if (estado == 1) // <-- Usa el 'estado' del rol
                {
                    e.Value = Properties.Resources.delete;
                    dataGridInquilinos.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Desactivar inquilino";
                }
                else
                {
                    e.Value = Properties.Resources.reactive;
                    dataGridInquilinos.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Reactivar inquilino";
                }
            }

            if (columnName == "ColumnaEstado")
            {
                e.Value = (estado == 1) ? "Activo" : "Inactivo"; // <-- Usa el 'estado' del rol
            }

            if (estado == 0) // <-- Usa el 'estado' del rol
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

        private void Inquilinos_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            CargarInquilinos();
        }

        private void CargarInquilinos()
        {
            dataGridInquilinos.Rows.Clear();
            dataGridInquilinos.AutoGenerateColumns = false;

            var negocio = new CN_PersonaRolCliente();
            string seleccion = comboBox1.SelectedItem?.ToString() ?? "Todos";
            EstadoFiltro filtro;

            switch (seleccion.ToUpper())
            {
                case "ACTIVOS":
                    filtro = EstadoFiltro.Activos;
                    break;
                case "INACTIVOS":
                    filtro = EstadoFiltro.Inactivos;
                    break;
                case "MOROSOS":
                    filtro = EstadoFiltro.Morosos;
                    break;
                default:
                    filtro = EstadoFiltro.Todos;
                    break;
            }

            _inquilinos = negocio.ListarClientes(TipoRolCliente.Inquilino, filtro);

            foreach (var i in _inquilinos)
            {
                dataGridInquilinos.Rows.Add(new object[]
                {
                    i.Dni,
                    i.oPersona?.Direccion,
                    i.oPersona?.Nombre,
                    i.oPersona?.Apellido,
                    i.oPersona?.Telefono,
                    i.oPersona?.CorreoElectronico,
                    
                    // ----- MODIFICADO -----
                    // Usamos el estado del rol (PersonaRolCliente.Estado)
                    i.Estado,

                    null,
                    null
                });
            }

            ActualizarIndicadores();
        }

        private void ActualizarIndicadores()
        {
            var estadisticas = new CN_PersonaRolCliente().ObtenerEstadisticas(TipoRolCliente.Inquilino);

            LTotalInquilinos.Text = estadisticas.Total.ToString();
            LInquilinosActivos.Text = estadisticas.Activos.ToString();
            LInquilinosInactivos.Text = estadisticas.Inactivos.ToString();
            LInquilinosMorosos.Text = "0";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarInquilinos();
        }
    }
}