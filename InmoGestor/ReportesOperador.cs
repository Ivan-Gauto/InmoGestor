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
    public partial class ReportesOperador : Form
    {
        public ReportesOperador()
        {
            InitializeComponent();
        }

        private void comboRol_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Paso 1: Obtener el texto del item seleccionado en el ComboBox.
            string reporteSeleccionado = comboBox2.SelectedItem?.ToString();

            // Si no hay nada seleccionado (raro, pero puede pasar), no hacer nada.
            if (string.IsNullOrEmpty(reporteSeleccionado))
            {
                return;
            }

            // Paso 2: Ocultar TODOS los DataGridViews.
            // Este es el paso clave para "limpiar" la vista antes de mostrar el correcto.
            dgvEstadoDeCuenta.Visible = false;
            dgvInquilinosMorosos.Visible = false;
            dgvInmueblesDisponibles.Visible = false;

            // Paso 3: Usar un switch (o if/else) para decidir qué DataGridView mostrar.
            switch (reporteSeleccionado)
            {
                case "Estado de Cuenta del Inquilino":
                    // Hacemos visible solo la grilla que corresponde.
                    dgvEstadoDeCuenta.Visible = true;
                    // También podrías deshabilitar filtros que no aplican a este reporte.
                    // Por ejemplo:
                    // txtSeleccionarInquilino.Enabled = true;
                    // txtSeleccionarContrato.Enabled = true;
                    break;

                case "Inquilinos Morosos":
                    dgvInquilinosMorosos.Visible = true;
                    // Para este reporte, quizás no necesites seleccionar un inquilino específico.
                    // txtSeleccionarInquilino.Enabled = false;
                    // txtSeleccionarContrato.Enabled = false;
                    break;

                case "Inmuebles Disponibles":
                    dgvInmueblesDisponibles.Visible = true;
                    // Aquí tampoco se necesitarían los filtros de inquilino/contrato.
                    // txtSeleccionarInquilino.Enabled = false;
                    // txtSeleccionarContrato.Enabled = false;
                    break;
            }

            // Nota: Aún no estamos cargando los datos, solo cambiando la visibilidad.
            // La carga de datos la harías al presionar "Generar vista previa".
        }
    }
}
