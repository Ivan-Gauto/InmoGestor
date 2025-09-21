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

        private void AbrirFormulario(Button boton, Form formulario)
        {
            // Restaurar color del botón anterior
            if (botonActivo != null)
                botonActivo.BackColor = Color.FromArgb(24, 123, 185);

            boton.BackColor = Color.FromArgb(0, 80, 200);
            botonActivo = boton;

            // Cerrar formulario activo previo
            if (formularioActivo != null)
                formularioActivo.Close();

            formularioActivo = formulario;

            // Configuración para que no altere el tamaño del padre
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            formulario.AutoSize = false;                    // 👈 clave

            // Limpiar contenedor antes de agregar
            Contenedor.Controls.Clear();                    // 👈 clave
            Contenedor.Controls.Add(formulario);

            formulario.BringToFront();
            formulario.Show();
        }


        private void BUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Usuarios());
        }

        private void BDashboard_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Dashboard());
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            AbrirFormulario(BDashboard, new Dashboard());
        }

        private void BInquilinos_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Inquilinos());
        }

        private void BPropietarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Propietarios());
        }

        private void BInmuebles_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Inmuebles());
        }
    }
}
