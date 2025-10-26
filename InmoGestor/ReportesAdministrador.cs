using CapaEntidades;
using CapaNegocio;
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
    public partial class ReportesAdministrador : Form
    {
        public ReportesAdministrador()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.ReportesAdministrador_Load);
            this.CBTipoReporte.SelectedIndexChanged += new System.EventHandler(this.CBTipoReporte_SelectedIndexChanged);
        }

        private void ReportesAdministrador_Load(object sender, EventArgs e)
        {
            CBTipoReporte.Items.Add("Auditoría de Usuarios");
            CBTipoReporte.Items.Add("Monitor de Errores");
            CBTipoReporte.SelectedIndex = 0;

            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            var negocioUsuario = new CN_Usuario();
            List<Usuario> listaUsuarios = negocioUsuario.Listar(RolUsuarioFiltro.Todos, EstadoFiltro.Activos);

            var usuarioTodos = new Usuario();
            usuarioTodos.Dni = "0";
            usuarioTodos.oPersona.Nombre = "Todos";
            usuarioTodos.oPersona.Apellido = "";
            listaUsuarios.Insert(0, usuarioTodos);

            CBUsuarios.DataSource = listaUsuarios;
            CBUsuarios.DisplayMember = "NombreCompleto";
            CBUsuarios.ValueMember = "Dni";
        }

        private void CBTipoReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            string seleccion = CBTipoReporte.SelectedItem.ToString();

            if (seleccion == "Auditoría de Usuarios")
            {
                dgvAuditoriaUsuarios.Visible = true;
                dgvErroresSistema.Visible = false;
            }
            else if (seleccion == "Monitor de Errores")
            {
                dgvAuditoriaUsuarios.Visible = false;
                dgvErroresSistema.Visible = true;
            }
        }
    }
}