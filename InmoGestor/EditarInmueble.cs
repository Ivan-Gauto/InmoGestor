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
using CapaEntidades;
using CapaNegocio;

namespace InmoGestor
{
    public partial class EditarInmueble : Form
    {
        private Inmueble _inmueble;
        private string _selectedImagePath;
        private readonly CN_Inmueble _cnInmueble = new CN_Inmueble();
        private readonly CN_PersonaRolCliente _cnPropietarios = new CN_PersonaRolCliente();

        public EditarInmueble()
        {
            InitializeComponent();

            // Asociar handlers que el diseñador no tenía ligados
            this.BIngresar.Click += BIngresar_Click;
            this.BCerrarForm.Click += BCerrarForm_Click;
            this.button1.Click += button1_Click;
        }

        // Nuevo constructor para abrir el formulario con un inmueble existente
        public EditarInmueble(Inmueble inmueble) : this()
        {
            _inmueble = inmueble ?? throw new ArgumentNullException(nameof(inmueble));
            CargarListas();
            CargarDatosEnFormulario();
        }

        private void CargarListas()
        {
            // Cargar tipos de inmueble
            try
            {
                var tipos = _cnInmueble.ListarTiposInmueble();
                cboTipoInmuebles.DisplayMember = "Nombre";
                cboTipoInmuebles.ValueMember = "IdTipoInmueble";
                cboTipoInmuebles.DataSource = tipos;
            }
            catch
            {
                cboTipoInmuebles.DataSource = null;
            }

            // Cargar propietarios
            try
            {
                var propietarios = _cnPropietarios.ListarClientes(CapaEntidades.TipoRolCliente.Propietario, CapaEntidades.EstadoFiltro.Activos);
                cboPropietarios.DisplayMember = "NombreCompleto";
                cboPropietarios.ValueMember = "Dni";
                cboPropietarios.DataSource = propietarios;
            }
            catch
            {
                cboPropietarios.DataSource = null;
            }
        }

        private void CargarDatosEnFormulario()
        {
            if (_inmueble == null) return;

            TDireccion.Text = _inmueble.Direccion;
            TDescripcion.Text = _inmueble.Descripcion;

            // Seleccionar tipo
            if (_inmueble.oTipoInmueble != null && cboTipoInmuebles.DataSource != null)
            {
                var tipos = (List<CapaEntidades.TipoInmueble>)cboTipoInmuebles.DataSource;
                var sel = tipos.FirstOrDefault(t => t.IdTipoInmueble == _inmueble.oTipoInmueble.IdTipoInmueble);
                if (sel != null) cboTipoInmuebles.SelectedItem = sel;
            }

            // Seleccionar propietario
            if (_inmueble.oPropietario != null && cboPropietarios.DataSource != null)
            {
                var props = (List<CapaEntidades.PersonaRolCliente>)cboPropietarios.DataSource;
                var selp = props.FirstOrDefault(p => p.Dni == _inmueble.oPropietario.Dni);
                if (selp != null) cboPropietarios.SelectedItem = selp;
            }

            // Cargar imagen si existe ruta
            if (!string.IsNullOrEmpty(_inmueble.RutaImagen))
            {
                try
                {
                    string rutaBase = Application.StartupPath;
                    string rutaCompleta = Path.Combine(rutaBase, "ImagenesInmuebles", _inmueble.RutaImagen);
                    if (File.Exists(rutaCompleta))
                    {
                        using (var fs = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read))
                        {
                            pictureBox1.Image = Image.FromStream(fs);
                        }
                    }
                }
                catch
                {
                    // ignorar y dejar imagen nula
                }
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar una foto";
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Guardar la ruta seleccionada para copiarla al guardar
                    _selectedImagePath = ofd.FileName;

                    // Cargar la imagen en memoria (no queda bloqueada en disco)
                    using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                    {
                        pictureBox1.Image = Image.FromStream(fs);
                    }
                }
            }
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(TDireccion.Text))
            {
                MessageBox.Show("La dirección es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboTipoInmuebles.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tipo de inmueble.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboPropietarios.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un propietario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Construir objeto Inmueble a actualizar
            var inmuebleActualizar = new Inmueble
            {
                IdInmueble = _inmueble.IdInmueble,
                Direccion = TDireccion.Text.Trim(),
                Descripcion = TDescripcion.Text?.Trim(),
                oTipoInmueble = (CapaEntidades.TipoInmueble)cboTipoInmuebles.SelectedItem,
                oPropietario = (CapaEntidades.PersonaRolCliente)cboPropietarios.SelectedItem,
                Disponibilidad = _inmueble.Disponibilidad,
                Division = _inmueble.Division,
                Estado = _inmueble.Estado
            };

            // Si se seleccionó una nueva imagen, copiarla a la carpeta de imágenes y asignar nombre
            if (!string.IsNullOrEmpty(_selectedImagePath) && File.Exists(_selectedImagePath))
            {
                try
                {
                    string carpeta = Path.Combine(Application.StartupPath, "ImagenesInmuebles");
                    if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

                    string nombreArchivo = Path.GetFileName(_selectedImagePath);
                    string destino = Path.Combine(carpeta, nombreArchivo);

                    // Copiar sobrescribiendo si existe
                    File.Copy(_selectedImagePath, destino, true);

                    inmuebleActualizar.RutaImagen = nombreArchivo;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al copiar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                // Mantener la ruta anterior si no se cambió la imagen
                inmuebleActualizar.RutaImagen = _inmueble.RutaImagen;
            }

            // Llamar a la capa de negocio para actualizar
            string mensaje;
            bool exito = _cnInmueble.Actualizar(inmuebleActualizar, out mensaje);

            if (exito)
            {
                MessageBox.Show("Inmueble actualizado correctamente.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
