using CapaEntidades;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InmoGestor
{
    public partial class InmuebleCard : UserControl
    {
        private Inmueble _inmueble;
        private const string _reactivarResourceName = "reactive";

        public InmuebleCard()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        public void CargarDatos(Inmueble inmueble)
        {
            _inmueble = inmueble;

            LDireccion.Text = inmueble.Direccion;
            LTipo.Text = inmueble.TipoInmuebleNombre;
            LEstado.Text = inmueble.DisponibilidadNombre;

            // --- INICIO DE LA MODIFICACIÓN ---

            // Lógica para el Propietario (LO QUE PIDES)
            // Comprobamos que el propietario y la persona no sean nulos
            if (inmueble.oPropietario != null &&
                inmueble.oPropietario.oPersona != null &&
                !string.IsNullOrEmpty(inmueble.oPropietario.oPersona.Nombre))
            {
                LPropietarioNombre.Text = $"{inmueble.oPropietario.oPersona.Nombre}, {inmueble.oPropietario.oPersona.Apellido}";
            }
            else
            {
                LPropietarioNombre.Text = "Sin propietario";
            }

            // --- FIN DE LA MODIFICACIÓN ---

            try
            {
                string rutaRelativa = inmueble.RutaImagen;
                if (!string.IsNullOrEmpty(rutaRelativa))
                {
                    string rutaBase = Application.StartupPath;
                    string rutaCompleta = Path.Combine(rutaBase, rutaRelativa);
                    if (File.Exists(rutaCompleta))
                    {
                        byte[] bytes = File.ReadAllBytes(rutaCompleta);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            PFotoInmueble.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        CargarImagenPorDefecto();
                    }
                }
                else
                {
                    CargarImagenPorDefecto();
                }
            }
            catch (Exception)
            {
                CargarImagenPorDefecto();
            }

            Color colorTextoPrincipal;
            Color colorTextoSecundario;
            Color colorFondoEstado;

            // --- LÓGICA DE COLORES MODIFICADA ---

            if (inmueble.Estado == 0) // Inactivo
            {
                this.BackColor = Color.DarkOrange;
                colorFondoEstado = Color.OrangeRed; // Más oscuro que el fondo
                LEstado.Text = "Inactivo";

                colorTextoPrincipal = Color.White;
                colorTextoSecundario = Color.White;
                // Cambiar icono a recurso de reactivación si existe
                var imgReact = GetResourceImage(_reactivarResourceName);
                if (imgReact != null) BEliminar.Image = imgReact;
            }
            else // Activo
            {
                this.BackColor = ColorTranslator.FromHtml("#22364F"); // Color por defecto

                colorTextoPrincipal = Color.White;
                colorTextoSecundario = ColorTranslator.FromHtml("#51B7D9");

                if (inmueble.Disponibilidad == 0) // Ocupado
                {
                    colorFondoEstado = Color.DarkOrange;
                }
                else if (inmueble.Disponibilidad == 1) // Disponible
                {
                    colorFondoEstado = ColorTranslator.FromHtml("#51B7D9");
                }
                else
                {
                    colorFondoEstado = Color.Gray;
                }

                btnEditar.Enabled = true;
                BEliminar.Enabled = true;
                try
                {
                    BEliminar.Image = Properties.Resources.delete;
                }
                catch
                {
                    // si no existe 'delete' en resources, no hacer nada
                }
            }

            LEstado.BackColor = colorFondoEstado;
            LDireccion.ForeColor = colorTextoPrincipal;
            LTipo.ForeColor = colorTextoPrincipal;
            LPropietarioNombre.ForeColor = colorTextoPrincipal;
            LInquilinoNombre.ForeColor = colorTextoPrincipal;
            LPrecio.ForeColor = colorTextoPrincipal;
            label1.ForeColor = colorTextoSecundario;
            label2.ForeColor = colorTextoSecundario;
            label3.ForeColor = colorTextoSecundario;
            LInquilinoNombre.Text = "Sin inquilino";
            LPrecio.Text = "$0";
        }

        private Image GetResourceImage(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName)) return null;
            try
            {
                var obj = Properties.Resources.ResourceManager.GetObject(resourceName);
                return obj as Image;
            }
            catch
            {
                return null;
            }
        }

        private void BEliminar_Click(object sender, EventArgs e)
        {
            if (_inmueble == null) return;

            CN_Inmueble negocioInmueble = new CN_Inmueble();
            bool exito = false;
            string accionUi;
            string msgExito;

            try
            {
                if (_inmueble.Estado == 0) // Lógica de REACTIVAR
                {
                    accionUi = "reactivar";
                    msgExito = "Inmueble reactivado correctamente.";
                    var dialogResult = MessageBox.Show(
                        $"¿Está seguro de que desea reactivar el inmueble en '{_inmueble.Direccion}'?",
                        "Confirmar Reactivación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (dialogResult != DialogResult.Yes) return;
                    exito = negocioInmueble.ReactivarLogico(_inmueble.IdInmueble);
                }
                else // Lógica de DESACTIVAR
                {
                    accionUi = "desactivar";
                    msgExito = "Inmueble desactivado correctamente.";
                    var dialogResult = MessageBox.Show(
                        $"¿Está seguro de que desea desactivar el inmueble en '{_inmueble.Direccion}'?",
                        "Confirmar Desactivación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (dialogResult != DialogResult.Yes) return;
                    exito = negocioInmueble.DesactivarLogico(_inmueble.IdInmueble);
                }

                if (exito)
                {
                    MessageBox.Show(msgExito, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var parentForm = this.FindForm() as Inmuebles;
                    parentForm?.RefrescarLista();
                }
                else
                {
                    MessageBox.Show($"No se pudo {accionUi} el inmueble.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarImagenPorDefecto()
        {
            PFotoInmueble.Image = Properties.Resources.noimage;
        }

        private void BEliminar_MouseEnter(object sender, EventArgs e)
        {
            // Si el inmueble está desactivado, evitamos poner rojo en hover y usamos un verde más claro
            if (_inmueble != null && _inmueble.Estado == 0)
            {
                BEliminar.BackColor = Color.LimeGreen;
                BEliminar.ForeColor = Color.White;
                return;
            }

            BEliminar.BackColor = ColorTranslator.FromHtml("red");
            BEliminar.ForeColor = Color.White;
        }

        private void BEliminar_MouseLeave(object sender, EventArgs e)
        {
            BEliminar.BackColor = Color.Transparent;
            BEliminar.ForeColor = Color.White;
        }

        private void btnEditar_MouseEnter(object sender, EventArgs e)
        {
            if (_inmueble != null && _inmueble.Estado == 0)
            {
                btnEditar.BackColor = Color.OrangeRed;
                return;
            }

            btnEditar.BackColor = ColorTranslator.FromHtml("#33CCFF");
            btnEditar.ForeColor = Color.White;
        }

        private void btnEditar_MouseLeave(object sender, EventArgs e)
        {
            btnEditar.BackColor = Color.Transparent;
            btnEditar.ForeColor = Color.White;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (_inmueble == null) return;

            // Si este control está dentro del formulario Inmuebles, usar método público para abrir editor embebido
            var parentForm = this.FindForm() as Inmuebles;
            if (parentForm != null)
            {
                parentForm.AbrirEditor(_inmueble);
                return;
            }

            // Si no, abrir como diálogo modal
            using (var f = new EditarInmueble(_inmueble))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    // intentar refrescar si existe el formulario padre Inmuebles
                    var owner = this.FindForm() as Inmuebles;
                    owner?.RefrescarLista();
                }
            }
        }
    }
}