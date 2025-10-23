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
using CapaNegocio;
using CapaEntidades;

namespace InmoGestor
{
    public partial class AgregarInmueble : Form
    {
        public event EventHandler InmuebleRegistradoConExito;
        private string rutaRelativaImagen = null;

        public AgregarInmueble()
        {
            InitializeComponent();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar una foto";
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 1. Define la carpeta de destino (relativa al ejecutable)
                        string carpetaDestino = "ImagenesInmuebles";

                        // 2. Crea la ruta completa (ej: C:\Proyecto\bin\Debug\ImagenesInmuebles)
                        string rutaCarpetaCompleta = Path.Combine(Application.StartupPath, carpetaDestino);

                        // 3. ¡Crea la carpeta si no existe!
                        Directory.CreateDirectory(rutaCarpetaCompleta);

                        // 4. Genera un nombre de archivo ÚNICO
                        // (Ej: "mi_foto.jpg" -> "9a8f...-456d-....jpg")
                        string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ofd.FileName);

                        // 5. Define la ruta de destino COMPLETA
                        string rutaDestinoCompleta = Path.Combine(rutaCarpetaCompleta, nombreArchivo);

                        // 6. ¡Copia el archivo!
                        File.Copy(ofd.FileName, rutaDestinoCompleta);

                        // 7. Guarda la RUTA RELATIVA (esto es lo que va a la BD)
                        this.rutaRelativaImagen = Path.Combine(carpetaDestino, nombreArchivo);

                        // 8. Muestra la imagen (cargándola desde su NUEVA ubicación)
                        using (var fs = new FileStream(rutaDestinoCompleta, FileMode.Open, FileAccess.Read))
                        {
                            pictureBox1.Image = Image.FromStream(fs);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.rutaRelativaImagen = null;
                        pictureBox1.Image = null;
                        MessageBox.Show("Error al copiar la imagen: " + ex.Message);
                    }
                }
            }
        }

        private void CargarPropietarios()
        {
            var negocioPersonas = new CN_PersonaRolCliente();
            List<PersonaRolCliente> listaPropietarios = negocioPersonas.ListarClientes(
                TipoRolCliente.Propietario,
                EstadoFiltro.Activos
            );
            cboPropietarios.DataSource = listaPropietarios;
            cboPropietarios.DisplayMember = "NombreCompleto";
            cboPropietarios.ValueMember = "Dni";
        }

        private void AgregarInmueble_Load(object sender, EventArgs e)
        {
            CargarPropietarios();
            CargarTiposInmueble();
        }

        private void CargarTiposInmueble()
        {
            var negocioInmueble = new CN_Inmueble();
            List<TipoInmueble> listaTipos = negocioInmueble.ListarTiposInmueble();
            cboTipoInmuebles.DataSource = listaTipos;
            cboTipoInmuebles.DisplayMember = "Nombre";
            cboTipoInmuebles.ValueMember = "IdTipoInmueble";
            cboTipoInmuebles.SelectedIndex = -1;
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TDireccion.Text))
            {
                MessageBox.Show("Debe ingresar una dirección.");
                return;
            }

            PersonaRolCliente propietarioSeleccionado = (PersonaRolCliente)cboPropietarios.SelectedItem;
            TipoInmueble tipoSeleccionado = (TipoInmueble)cboTipoInmuebles.SelectedItem;

            if (propietarioSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un propietario.");
                return;
            }
            if (tipoSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de inmueble.");
                return;
            }

            Inmueble nuevoInmueble = new Inmueble();
            nuevoInmueble.Direccion = TDireccion.Text;
            nuevoInmueble.Descripcion = TDescripcion.Text;
            nuevoInmueble.oPropietario = propietarioSeleccionado;
            nuevoInmueble.oTipoInmueble = tipoSeleccionado;
            nuevoInmueble.Disponibilidad = 1;
            nuevoInmueble.Estado = 1;
            nuevoInmueble.RutaImagen = this.rutaRelativaImagen;

            string mensaje = string.Empty;
            bool exito = new CN_Inmueble().Registrar(nuevoInmueble, out mensaje);

            if (exito)
            {
                MessageBox.Show("Inmueble registrado con éxito.");
                InmuebleRegistradoConExito?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al registrar: \n" + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}