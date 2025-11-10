using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidades; // <-- Requerido
using CapaNegocio; // <-- Requerido

namespace InmoGestor
{
    public partial class Propietarios : Form
    {
        private EditarPropietario editarPropietarioForm;
        private AgregarPropietario agregarPropietarioForm;

        // 1. Lista para almacenar los datos, igual que _inquilinos
        private List<PersonaRolCliente> _propietarios = new List<PersonaRolCliente>();

        public Propietarios()
        {
            InitializeComponent();
            this.Resize += Propietarios_Resize;
        }

        #region "Gestión de Formularios Hijos (Pop-ups)"
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

            // (Asegúrate que tu panel contenedor se llame 'ContenedorPropietarios')
            f.Location = new Point(
                (ContenedorPropietarios.Width - f.Width) / 2,
                (ContenedorPropietarios.Height - f.Height) / 2
            );
        }
        #endregion

        #region "Métodos de Carga y Actualización"

        // 2. Evento Load (Modificado)
        private void Propietarios_Load(object sender, EventArgs e)
        {
            comboBox3.SelectedIndex = 0;
            CargarPropietarios(); // Esto dispara CargarPropietarios()
        }

        // 3. Método CargarPropietarios (Análogo a CargarInquilinos)
        private void CargarPropietarios()
        {
            dataGridPropietarios.Rows.Clear();
            dataGridInmuebles.Rows.Clear();
            dataGridPropietarios.AutoGenerateColumns = false;

            var negocio = new CN_PersonaRolCliente();
            // (Usamos 'comboBox1' como en Inquilinos)
            string seleccion = comboBox3.SelectedItem?.ToString() ?? "Todos";
            EstadoFiltro filtro;

            switch (seleccion.ToUpper())
            {
                case "ACTIVOS":
                    filtro = EstadoFiltro.Activos;
                    break;
                case "INACTIVOS":
                    filtro = EstadoFiltro.Inactivos;
                    break;
                default:
                    filtro = EstadoFiltro.Todos;
                    break;
            }

            // --- Reutilización de la Capa de Negocio ---
            _propietarios = negocio.ListarClientes(TipoRolCliente.Propietario, filtro);

            // 4. Poblar el grid (ajustado a tus 11 columnas)
            foreach (var p in _propietarios)
            {
                dataGridPropietarios.Rows.Add(new object[]
                {
                    p.Dni,                          // 1. DNI
                    p.oPersona?.Direccion,          // 2. Direccion
                    p.oPersona?.Nombre,             // 3. Nombre
                    p.oPersona?.Apellido,           // 4. Apellido
                    p.oPersona?.Telefono,           // 5. Telefono
                    p.oPersona?.CorreoElectronico,  // 6. Correo
                    p.Estado,                       // 9. Estado (del rol)
                    null,                           // 10. Editar (para el icono)
                    null                            // 11. Accion (para el icono)
                });
            }
            ActualizarIndicadores();
        }

        // 5. Método ActualizarIndicadores (Análogo a Inquilinos)
        private void ActualizarIndicadores()
        {
            var estadisticas = new CN_PersonaRolCliente().ObtenerEstadisticas(TipoRolCliente.Propietario);

            LTotalPropietarios.Text = estadisticas.Total.ToString();
            LPropietariosActivos.Text = estadisticas.Activos.ToString();
            LPropietariosInactivos.Text = estadisticas.Inactivos.ToString();
        }

        // 6. Evento del ComboBox (Análogo a Inquilinos)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarPropietarios();
        }

        private void CargarInmuebles(string dniPropietario)
        {
            dataGridInmuebles.Rows.Clear();
            dataGridInmuebles.AutoGenerateColumns = false;

            if (string.IsNullOrEmpty(dniPropietario))
            {
                return;
            }

            try
            {
                var negocioInmueble = new CN_Inmueble();
                List<Inmueble> inmuebles = negocioInmueble.ListarPorPropietario(dniPropietario, (int)TipoRolCliente.Propietario);

                foreach (var inm in inmuebles)
                {
                    dataGridInmuebles.Rows.Add(new object[]
                    {
                inm.Direccion,
                inm.Descripcion,
                // --- AQUÍ ESTÁ LA CORRECCIÓN ---
                // Accedemos a 'oPersona' que está dentro de 'oPropietario'
                inm.oPropietario?.oPersona?.Nombre + " " + inm.oPropietario?.oPersona?.Apellido,
                inm.oTipoInmueble?.Nombre,
                inm.Estado // El estado numérico
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar inmuebles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region "Eventos del DataGridView"

        private void dataGridPropietarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridPropietarios.CurrentRow == null || dataGridPropietarios.CurrentRow.Index < 0)
            {
                CargarInmuebles(null); // Limpiar grilla de inmuebles si no hay selección
                return;
            }

            // Obtener el DNI de la fila seleccionada
            string dniPropietario = dataGridPropietarios.CurrentRow.Cells["ColumnaDni"].Value?.ToString();

            // Llamar al nuevo método para cargar los inmuebles
            CargarInmuebles(dniPropietario);
        }
        private void dataGridPropietarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            // Usamos los nombres de columna de tu imagen
            var colName = dataGridPropietarios.Columns[e.ColumnIndex].Name;

            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            if (colName == "ColumnaEditar") // <-- Nombre de tu columna
            {
                var dniEditar = dataGridPropietarios.Rows[e.RowIndex].Cells["ColumnaDni"].Value?.ToString(); // <-- Nombre de tu columna
                var pSel = _propietarios.FirstOrDefault(x => x.Dni == dniEditar);

                if (pSel == null)
                {
                    MessageBox.Show("No se encontró el propietario.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // (Asumo que tu form se llama 'EditarPropietario')
                editarPropietarioForm = new EditarPropietario(pSel);

                editarPropietarioForm.TopLevel = false;
                editarPropietarioForm.FormBorderStyle = FormBorderStyle.None;
                ContenedorPropietarios.Controls.Add(editarPropietarioForm);

                editarPropietarioForm.FormClosed += (_, __) =>
                {
                    ContenedorPropietarios.Controls.Remove(editarPropietarioForm);
                    editarPropietarioForm.Dispose();
                    editarPropietarioForm = null;
                    CargarPropietarios(); // Recargar al cerrar
                };

                editarPropietarioForm.Show();
                editarPropietarioForm.BringToFront();
                editarPropietarioForm.Focus();
                CentrarFormAbierto();
            }
            else if (colName == "ColumnaAcciones") // <-- Nombre de tu columna
            {
                try
                {
                    string dni = dataGridPropietarios.Rows[e.RowIndex].Cells["ColumnaDni"].Value.ToString(); // <-- Nombre de tu columna
                    var propSeleccionado = _propietarios.FirstOrDefault(p => p.Dni == dni);
                    if (propSeleccionado == null) return;

                    int estadoActual = propSeleccionado.Estado;
                    int idRolPropietario = (int)TipoRolCliente.Propietario;

                    if (estadoActual == 1)
                    {
                        DialogResult resultado = MessageBox.Show("¿Está seguro de que desea desactivar a este propietario?", "Confirmar Baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                        {
                            if (new CN_PersonaRolCliente().CambiarEstado(dni, 0, idRolPropietario))
                            {
                                CargarPropietarios();
                            }
                        }
                    }
                    else
                    {
                        DialogResult resultado = MessageBox.Show("¿Desea reactivar este propietario?", "Confirmar Reactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                        {
                            if (new CN_PersonaRolCliente().CambiarEstado(dni, 1, idRolPropietario))
                            {
                                CargarPropietarios();
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

        // 8. Evento CellFormatting (Para iconos y colores)
        private void dataGridPropietarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string dni = dataGridPropietarios.Rows[e.RowIndex].Cells["ColumnaDni"].Value?.ToString();
            if (string.IsNullOrEmpty(dni)) return;

            PersonaRolCliente propActual = _propietarios.FirstOrDefault(p => p.Dni == dni);
            if (propActual == null)
            {
                // Si el DNI está en la grilla pero no en la lista (raro), salimos
                return;
            }

            int estado = propActual.Estado;
            string columnName = dataGridPropietarios.Columns[e.ColumnIndex].Name;

            if (columnName == "ColumnaEditar") // <-- Nombre de tu columna
            {
                e.Value = Properties.Resources.edit;
                dataGridPropietarios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Editar propietario";
            }

            if (columnName == "ColumnaAcciones") // <-- Nombre de tu columna
            {
                if (estado == 1)
                {
                    e.Value = Properties.Resources.delete;
                    dataGridPropietarios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Desactivar propietario";
                }
                else
                {
                    e.Value = Properties.Resources.reactive;
                    dataGridPropietarios.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Reactivar propietario";
                }
            }

            if (columnName == "ColumnaEstado") // <-- Nombre de tu columna
            {
                e.Value = (estado == 1) ? "Activo" : "Inactivo";
            }

            // Pintar la fila si está inactivo
            if (estado == 0)
            {
                e.CellStyle.BackColor = Color.DarkOrange;
                e.CellStyle.ForeColor = Color.White;
            }
            else
            {
                // (Asumo este color de fondo por tu imagen)
                e.CellStyle.BackColor = Color.FromArgb(15, 30, 45);
                e.CellStyle.ForeColor = Color.White;
            }
        }

        private void dataGridInmuebles_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Asumo que tu columna de estado en la grilla de inmuebles se llama 'ColumnaEstadoInmueble'
            // ¡Verifica este nombre!
            string columnName = dataGridInmuebles.Columns[e.ColumnIndex].Name;

            if (columnName == "ColumnaEstadoInmueble")
            {
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int estado))
                {
                    e.Value = (estado == 1) ? "Activo" : "Inactivo";
                    e.FormattingApplied = true;
                }
            }

            // Pintar toda la fila según el estado
            var cellEstado = dataGridInmuebles.Rows[e.RowIndex].Cells["ColumnaEstadoInmueble"].Value;
            if (cellEstado != null && int.TryParse(cellEstado.ToString(), out int estadoFila))
            {
                if (estadoFila == 0)
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

        #endregion

        // 9. Evento clic del botón Agregar (Análogo a Inquilinos)
        private void BAgregarPropietario_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            // (Asumo que tu form se llama 'AgregarPropietario')
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
                CargarPropietarios(); // <-- Recargar al cerrar
            };

            agregarPropietarioForm.Show();
            agregarPropietarioForm.BringToFront();
            agregarPropietarioForm.Focus();
            CentrarFormAbierto();
        }

    }
}