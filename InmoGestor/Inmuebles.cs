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
    public partial class Inmuebles : Form
    {
        private EditarInmueble editarInmuebleForm;
        private AgregarInmueble agregarInmuebleForm;

        public Inmuebles()
        {
            InitializeComponent();
            this.Load += Inmuebles_Load;
            this.Resize += Inmueble_Resize;

            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                ContenedorInmuebles,
                new object[] { true });
        }

        private static bool IsOpen(Form f) => f != null && !f.IsDisposed && f.Visible;
        private bool HayFormAbierto() =>
            IsOpen(agregarInmuebleForm) || IsOpen(editarInmuebleForm);

        private void FocusFormAbierto()
        {
            if (IsOpen(agregarInmuebleForm)) { agregarInmuebleForm.BringToFront(); agregarInmuebleForm.Focus(); return; }
            if (IsOpen(editarInmuebleForm)) { editarInmuebleForm.BringToFront(); editarInmuebleForm.Focus(); return; }

        }

        private void Inmueble_Resize(object sender, EventArgs e) => CentrarFormAbierto();

        private void CentrarFormAbierto()
        {
            Form f = IsOpen(agregarInmuebleForm) ? (Form)agregarInmuebleForm
                  : IsOpen(editarInmuebleForm) ? (Form)editarInmuebleForm
                  : null;

            if (f == null) return;

            f.Location = new Point(
                (ContenedorInmuebles.Width - f.Width) / 2,
                (ContenedorInmuebles.Height - f.Height) / 2
            );
        }

        private void BAgregarInmueble_Click(object sender, EventArgs e)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            agregarInmuebleForm = new AgregarInmueble
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorInmuebles.Controls.Add(agregarInmuebleForm);

            agregarInmuebleForm.InmuebleRegistradoConExito += (s, ev) =>
            {
                CargarInmuebles();
            };

            agregarInmuebleForm.FormClosed += (_, __) =>
            {
                ContenedorInmuebles.Controls.Remove(agregarInmuebleForm);
                agregarInmuebleForm.Dispose();
                agregarInmuebleForm = null;
            };

            agregarInmuebleForm.Show();
            agregarInmuebleForm.BringToFront();
            agregarInmuebleForm.Focus();
            CentrarFormAbierto();
        }

        private void Inmuebles_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            CargarInmuebles();
            ActualizarContadores();
        }

        private void CargarInmuebles()
        {
            flowLayoutPanelInmuebles.Controls.Clear();

            CN_Inmueble negocioInmueble = new CN_Inmueble();

            // 1. Determinar el filtro seleccionado
            string filtro = comboBox1.SelectedItem?.ToString() ?? "Activos";

            List<Inmueble> listaParaMostrar;

            // 2. Cargar y filtrar la lista según el caso
            if (filtro == "Inactivos")
            {
                // Traemos TODOS (activos e inactivos) y filtramos solo los inactivos
                listaParaMostrar = negocioInmueble.Listar(true)
                                                  .Where(i => i.Estado == 0)
                                                  .ToList();
            }
            else
            {
                // Traemos solo los ACTIVOS (default)
                var listaActivos = negocioInmueble.Listar(false);

                if (filtro == "Disponibles")
                {
                    listaParaMostrar = listaActivos.Where(i => i.Disponibilidad == 1).ToList();
                }
                else if (filtro == "Ocupados")
                {
                    listaParaMostrar = listaActivos.Where(i => i.Disponibilidad == 0).ToList();
                }
                else // "Activos" o cualquier caso por defecto
                {
                    listaParaMostrar = listaActivos;
                }
            }

            // 3. (El resto de tu método sigue igual)
            foreach (Inmueble inmueble in listaParaMostrar)
            {
                InmuebleCard card = new InmuebleCard();
                card.CargarDatos(inmueble);
                flowLayoutPanelInmuebles.Controls.Add(card);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cada vez que cambia el filtro, volvemos a cargar las tarjetas
            CargarInmuebles();
        }

        private void ActualizarContadores()
        {
            // 1. Obtenemos la lista completa de inmuebles
            List<Inmueble> listaInmuebles = new CN_Inmueble().Listar();

            // 2. Usamos LINQ para contar según la disponibilidad
            // (Asumiendo 1 = Disponible, 0 = Ocupado/No Disponible)

            int totalDisponibles = listaInmuebles.Count(inmueble => inmueble.Disponibilidad == 1);
            int totalOcupados = listaInmuebles.Count(inmueble => inmueble.Disponibilidad == 0);
            int totalInactivos = listaInmuebles.Count(inmueble => inmueble.Estado == 0);

            // 3. Actualizamos el texto de los Labels
            LDisponibles.Text = totalDisponibles.ToString();
            LOcupados.Text = totalOcupados.ToString();
            LInactivos.Text = totalInactivos.ToString();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        // Abre el editor embebido (misma lógica que agregación) para editar el inmueble
        public void AbrirEditor(CapaEntidades.Inmueble inmueble)
        {
            if (HayFormAbierto()) { FocusFormAbierto(); return; }

            editarInmuebleForm = new EditarInmueble(inmueble)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            ContenedorInmuebles.Controls.Add(editarInmuebleForm);

            editarInmuebleForm.FormClosed += (_, __) =>
            {
                ContenedorInmuebles.Controls.Remove(editarInmuebleForm);
                editarInmuebleForm.Dispose();
                editarInmuebleForm = null;

                // refrescar lista siempre que el editor se cierre
                CargarInmuebles();
                ActualizarContadores();
            };

            editarInmuebleForm.Show();
            editarInmuebleForm.BringToFront();
            editarInmuebleForm.Focus();
            CentrarFormAbierto();
        }

        // Método público para refrescar desde controles hijos
        public void RefrescarLista()
        {
            CargarInmuebles();
            ActualizarContadores();
        }
    }
}