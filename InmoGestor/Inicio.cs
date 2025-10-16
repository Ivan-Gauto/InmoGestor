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


namespace InmoGestor
{
    public partial class Inicio : Form
    {
        private static Button botonActivo = null;
        private static Form formularioActivo = null;

        private static Usuario usuarioActual;

        // Constantes de rol (según tu BD)
        private const int ROL_ADMIN = 1; // accede a todo
        private const int ROL_OPERADOR = 2; // todo MENOS Usuarios
        private const int ROL_AYUDANTE = 3; // como Operador, y además SIN Reportes


        public Inicio(Usuario oUsuario)
        {
            usuarioActual = oUsuario;

            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {            
            LBUsuario.Text = usuarioActual.NombreCompleto;

            // Limitar menú según rol
            AplicarPermisos();

            AbrirFormulario(BDashboard, new Dashboard());

        }

        private void AplicarPermisos()
        {
            int rol = usuarioActual?.RolUsuarioId ?? 0;

            // Todos ven esto:
            BDashboard.Visible = true;
            BInquilinos.Visible = true;
            BPropietarios.Visible = true;
            BInmuebles.Visible = true;
            button11.Visible = true; // Contratos

            // Usuarios: solo Admin
            BUsuarios.Visible = (rol == ROL_ADMIN);
            LBAdministracion.Visible = (rol == ROL_ADMIN);

            // Reportes: Admin y Operador (Ayudante no)
            button3.Visible = (rol == ROL_ADMIN || rol == ROL_OPERADOR);

        }

        // ---------- Guardas extra en los clicks (defensa en profundidad) ----------
        private void BUsuarios_Click(object sender, EventArgs e)
        {
            if (usuarioActual?.RolUsuarioId != ROL_ADMIN)
            {
                MessageBox.Show("No tiene permisos para acceder a Usuarios.", "Permisos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario((Button)sender, new Usuarios());
        }

        private void button3_Click(object sender, EventArgs e) // Reportes
        {
            if (usuarioActual?.RolUsuarioId == ROL_AYUDANTE)
            {
                MessageBox.Show("No tiene permisos para acceder a Reportes.", "Permisos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario((Button)sender, new Reportes());
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

        private void BDashboard_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Dashboard());
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

        private void button11_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Contratos());
        }

    }
}
