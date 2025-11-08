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

        private const int ROL_ADMIN = 1;
        private const int ROL_OPERADOR = 3;
        private const int ROL_AYUDANTE = 0;
        private const int ROL_GERENTE = 2;

        public Inicio(Usuario oUsuario)
        {
            usuarioActual = oUsuario;
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            LBUsuario.Text = usuarioActual.NombreCompleto;
            AplicarPermisos();

            if (usuarioActual.RolUsuarioId == ROL_ADMIN)
            {
                AbrirFormulario(BUsuarios, new Usuarios(usuarioActual));
            }
            else
            {
                AbrirFormulario(BDashboard, new Dashboard());
            }
        }

        private void AplicarPermisos()
        {
            int rol = usuarioActual?.RolUsuarioId ?? 0;

            BDashboard.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR);
            BInquilinos.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR);
            BPropietarios.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR);
            BInmuebles.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR);
            BContratos.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR);
            BPagos.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR);
            BUsuarios.Enabled = (rol == ROL_ADMIN);
            BReportes.Enabled = (rol == ROL_GERENTE || rol == ROL_OPERADOR || rol == ROL_ADMIN);
            BBackup.Enabled = (rol == ROL_ADMIN);
        }

        private void BUsuarios_Click(object sender, EventArgs e)
        {
            if (usuarioActual?.RolUsuarioId != ROL_ADMIN)
            {
                MessageBox.Show("No tiene permisos para acceder a Usuarios.", "Permisos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario((Button)sender, new Usuarios(usuarioActual));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int rol = usuarioActual?.RolUsuarioId ?? 0;
            switch (rol)
            {
                case ROL_ADMIN:
                    AbrirFormulario((Button)sender, new ReportesAdministrador());
                    break;
                case ROL_GERENTE:
                    AbrirFormulario((Button)sender, new ReportesGerente());
                    break;
                case ROL_OPERADOR:
                    AbrirFormulario((Button)sender, new ReportesOperador());
                    break;
                default:
                    MessageBox.Show("No tienes permisos definidos para acceder a los reportes.",
                                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void AbrirFormulario(Button boton, Form formulario)
        {
            if (botonActivo != null)
                botonActivo.BackColor = Color.FromArgb(24, 123, 185);

            boton.BackColor = Color.FromArgb(0, 80, 200);
            botonActivo = boton;

            if (formularioActivo != null)
                formularioActivo.Close();

            formularioActivo = formulario;
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            formulario.AutoSize = false;

            Contenedor.Controls.Clear();
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
            AbrirFormulario((Button)sender, new Inquilinos(usuarioActual));
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

        private void BSalir_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea salir?", "Confirmar salida",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Pagos());
        }

        private void BBackup_Click(object sender, EventArgs e)
        {
            AbrirFormulario((Button)sender, new Backup());
        }
    }
}