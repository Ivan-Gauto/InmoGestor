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
        private AgregarUsuarios agregarUsuariosForm;

        public Usuarios()
        {
            InitializeComponent();
            this.Resize += Usuarios_Resize;
        }

        private void Usuarios_Resize(object sender, EventArgs e)
        {
            CentrarAgregarUsuarios();
        }

        private void CentrarAgregarUsuarios()
        {
            if (agregarUsuariosForm != null && agregarUsuariosForm.Visible)
            {
                agregarUsuariosForm.Location = new Point(
                    (ContenedorUsuarios.Width - agregarUsuariosForm.Width) / 2,
                    (ContenedorUsuarios.Height - agregarUsuariosForm.Height) / 2
                );
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_2(object sender, PaintEventArgs e)
        {

        }

        private void Usuarios_Load(object sender, EventArgs e)
        {
            // Limpia la tabla primero
            dataGridUsuarios.Rows.Clear();

            // Agrega filas de ejemplo
            dataGridUsuarios.Rows.Add("u001", "Av. Siempre Viva 742", "Juan", "Pérez", "123456789", "juan@mail.com", "Admin", "Activo");
            dataGridUsuarios.Rows.Add("u002", "Calle Falsa 123", "Ana", "Gómez", "987654321", "ana@mail.com", "Usuario", "Inactivo");


        }

        private void BAgregarUsuario_Click(object sender, EventArgs e)
        {
            agregarUsuariosForm = new AgregarUsuarios();

            agregarUsuariosForm.TopLevel = false;  
            agregarUsuariosForm.FormBorderStyle = FormBorderStyle.None;

            ContenedorUsuarios.Controls.Add(agregarUsuariosForm);

            agregarUsuariosForm.BringToFront();
            agregarUsuariosForm.Show();

            CentrarAgregarUsuarios();
        }

    }
}
