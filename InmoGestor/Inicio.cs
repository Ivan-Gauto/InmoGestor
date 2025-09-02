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
    public partial class Inicio : Form
    {
        private static Button botonActivo = null;
        private static Form formularioActivo = null;
        public Inicio()
        {
            InitializeComponent();
        }

        private void BDashboard_Click(object sender, EventArgs e)
        {

        }

        private void AbrirFormulario(Button boton, Form formulario)
        {
            if (botonActivo != null)
            {
                botonActivo.BackColor = Color.FromArgb(26, 32, 40);
            }

            boton.BackColor = Color.FromArgb(0, 80, 200);
            botonActivo = boton;

            if (formularioActivo != null)
            {
                formularioActivo.Close();
            }

            formularioActivo = formulario;
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None; 
            formulario.Dock = DockStyle.Fill;
            Contenedor.Controls.Add(formulario);
            formulario.Show();
        }

        private void BUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Usuarios());
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
