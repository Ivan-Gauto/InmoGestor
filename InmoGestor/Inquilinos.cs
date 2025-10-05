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
                // 1) tomar DNI desde la grilla
                var dni = dataGridInquilinos.Rows[e.RowIndex]
                                          .Cells["UsuarioColumna"] // <-- Asegurate que la columna DNI se llame así
                                          .Value?.ToString();

                // 2) buscar el objeto completo en la lista en memoria
                var iSel = _inquilinos.FirstOrDefault(x => x.Dni == dni);

                if (iSel == null)
                {
                    MessageBox.Show("No se encontró el inquilino.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (HayFormAbierto()) { FocusFormAbierto(); return; }

                // 3) abrir el editor PASÁNDOLE el usuario
                editarInquilinoForm = new EditarInquilino(iSel)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None
                };
                ContenedorInquilinos.Controls.Add(editarInquilinoForm);

                // refrescar al cerrar
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
        }

        private void Inquilinos_Load(object sender, EventArgs e)
        {
            CargarInquilinos();
        }

        private void CargarInquilinos()
        {
            dataGridInquilinos.Rows.Clear();
            dataGridInquilinos.AutoGenerateColumns = false;

            _inquilinos = new CN_PersonaRolCliente().ListarInquilinos(2)
                 ?? new List<PersonaRolCliente>();

            foreach (var i in _inquilinos)
            {
                dataGridInquilinos.Rows.Add(new object[]
                {
                i.Dni,
                i.oPersona?.Direccion ?? "",
                i.oPersona?.Nombre ?? "",
                i.oPersona?.Apellido ?? "",
                i.oPersona?.Telefono ?? "",
                i.oPersona?.CorreoElectronico ?? ""
                });
            }

        }
    }
}
