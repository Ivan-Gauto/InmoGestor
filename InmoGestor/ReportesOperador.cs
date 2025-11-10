using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CapaNegocio;
using CapaEntidades;

namespace InmoGestor
{
    public partial class ReportesOperador : Form
    {
        private CN_Reportes negocioReportes = new CN_Reportes();
        private CN_PersonaRolCliente negocioClientes = new CN_PersonaRolCliente();
        private CN_Contrato negocioContrato = new CN_Contrato();

        private List<PersonaRolCliente> inquilinosActivos = new List<PersonaRolCliente>();
        private List<ContratoAlquiler> contratosActivos = new List<ContratoAlquiler>();

        public ReportesOperador()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(this.ReportesOperador_Load);
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            this.BDescargarcvs.Click += new System.EventHandler(this.BDescargarcvs_Click);

            this.cmbSeleccionarInquilino.SelectedIndexChanged += new System.EventHandler(this.cmbSeleccionarInquilino_SelectedIndexChanged);
            this.cmbSeleccionarContrato.SelectedIndexChanged += new System.EventHandler(this.cmbSeleccionarContrato_SelectedIndexChanged);

            this.dtpInicio.ValueChanged += new System.EventHandler(this.Filtros_Reporte_Changed);
            this.dtpFin.ValueChanged += new System.EventHandler(this.Filtros_Reporte_Changed);
        }

        private void ReportesOperador_Load(object sender, EventArgs e)
        {
            ConfigurarGrids();

            CargarComboInquilinos();

            comboBox2.Items.Clear();
            comboBox2.Items.Add("Estado de Cuenta del Inquilino");
            comboBox2.Items.Add("Inquilinos Morosos");
            comboBox2.Items.Add("Inmuebles Disponibles");

            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
        }

        private void CargarComboInquilinos()
        {
            inquilinosActivos = negocioClientes.ListarClientes(TipoRolCliente.Inquilino, EstadoFiltro.Activos);

            var dsInquilinos = inquilinosActivos
                .Where(x => x.oPersona != null)
                .Select(x => new {
                    Dni = x.Dni,
                    DisplayText = $"{x.oPersona.Apellido}, {x.oPersona.Nombre} ({x.Dni})"
                })
                .ToList();

            cmbSeleccionarInquilino.ValueMember = "Dni";
            cmbSeleccionarInquilino.DisplayMember = "DisplayText";
            cmbSeleccionarInquilino.DataSource = dsInquilinos;

            if (dsInquilinos.Any())
            {
                cmbSeleccionarInquilino.SelectedIndex = 0;
            }
        }

        private void cmbSeleccionarInquilino_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSeleccionarInquilino.SelectedValue == null) return;

            string dniInquilino = cmbSeleccionarInquilino.SelectedValue.ToString();

            contratosActivos = negocioContrato.ListarContratos(estado: 1, dniInquilino: dniInquilino);

            var dsContratos = contratosActivos.Select(c => new {
                ContratoId = c.ContratoId,
                DisplayText = $"Contrato #{c.ContratoId} ({c.oInmueble.Direccion})"
            }).ToList();

            cmbSeleccionarContrato.ValueMember = "ContratoId";
            cmbSeleccionarContrato.DisplayMember = "DisplayText";
            cmbSeleccionarContrato.DataSource = dsContratos;

            if (dsContratos.Any())
            {
                cmbSeleccionarContrato.SelectedIndex = 0;
            }
            else
            {
                cmbSeleccionarContrato.DataSource = null;
                CargarEstadoDeCuenta();
            }
        }

        private void cmbSeleccionarContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarEstadoDeCuenta();
        }

        private void Filtros_Reporte_Changed(object sender, EventArgs e)
        {
            CargarEstadoDeCuenta();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            string reporteSeleccionado = comboBox2.SelectedItem.ToString();

            dgvEstadoDeCuenta.Visible = false;
            dgvInquilinosMorosos.Visible = false;
            dgvInmueblesDisponibles.Visible = false;

            cmbSeleccionarInquilino.Visible = false;
            cmbSeleccionarContrato.Visible = false;
            dtpInicio.Visible = false;
            dtpFin.Visible = false;

            switch (reporteSeleccionado)
            {
                case "Estado de Cuenta del Inquilino":
                    dgvEstadoDeCuenta.Visible = true;
                    cmbSeleccionarInquilino.Visible = true;
                    cmbSeleccionarContrato.Visible = true;
                    dtpInicio.Visible = true;
                    dtpFin.Visible = true;
                    CargarEstadoDeCuenta();
                    break;

                case "Inquilinos Morosos":
                    dgvInquilinosMorosos.Visible = true;
                    dtpInicio.Visible = true;
                    dtpFin.Visible = true;
                    break;

                case "Inmuebles Disponibles":
                    dgvInmueblesDisponibles.Visible = true;
                    break;
            }
        }

        private void CargarEstadoDeCuenta()
        {
            if (!dgvEstadoDeCuenta.Visible) return;

            if (cmbSeleccionarContrato.SelectedValue == null)
            {
                dgvEstadoDeCuenta.DataSource = null;
                return;
            }

            int contratoId = (int)cmbSeleccionarContrato.SelectedValue;
            DateTime fechaInicio = dtpInicio.Value.Date;
            DateTime fechaFin = dtpFin.Value.Date;

            DataTable dt = negocioReportes.ObtenerEstadoDeCuenta(contratoId, fechaInicio, fechaFin);
            dgvEstadoDeCuenta.DataSource = dt;
        }

        private void ConfigurarGrids()
        {
            dgvEstadoDeCuenta.AutoGenerateColumns = false;

            ColumnaPeriodo.DataPropertyName = "Periodo";
            ColumnaFecha.DataPropertyName = "Fecha";
            ColumnaDetalle.DataPropertyName = "DetalleConcepto";
            ColumnaDebe.DataPropertyName = "Debe";
            ColumnaHaber.DataPropertyName = "Haber";
            ColumnaEstadoPago.DataPropertyName = "EstadoDelPago";
        }

        private void Descargarcvs(DataGridView dgv)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos CSV (*.csv)|*.csv";
            sfd.FileName = "Reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        sb.Append(dgv.Columns[i].HeaderText + ";");
                    }
                    sb.Append("\r\n");

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                string valor = row.Cells[i].Value != null ? row.Cells[i].Value.ToString() : "";
                                valor = valor.Replace(";", ",").Replace("\n", " ").Replace("\r", "");
                                sb.Append(valor + ";");
                            }
                            sb.Append("\r\n");
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show("Reporte exportado exitosamente.", "Exportación CSV Exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BDescargarcvs_Click(object sender, EventArgs e)
        {
            if (dgvEstadoDeCuenta.Visible)
            {
                Descargarcvs(dgvEstadoDeCuenta);
            }
            else if (dgvInquilinosMorosos.Visible)
            {
                Descargarcvs(dgvInquilinosMorosos);
            }
            else if (dgvInmueblesDisponibles.Visible)
            {
                Descargarcvs(dgvInmueblesDisponibles);
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un reporte y genere la vista previa para exportar datos.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}