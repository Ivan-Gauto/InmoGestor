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
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

        private void BIngresar_Click(object sender, EventArgs e)
        {
            Inicio form = new Inicio();

            string dniText = TIngresoDNI.Text.Trim();
            string clave = TIngresoClave.Text.Trim();


            bool camposFaltantes = string.IsNullOrWhiteSpace(dniText) || string.IsNullOrWhiteSpace(clave);

            if (camposFaltantes)
            {
                MessageBox.Show("Debe completar todos los campos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(dniText, out int dni))
            {
                MessageBox.Show("El DNI debe ser numérico.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dniText.Length < 7 || dniText.Length > 8)
            {
                MessageBox.Show("El DNI debe tener 7 u 8 dígitos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            form.Show();
            this.Hide();

            form.FormClosing += frm_closing;
        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {   
            TIngresoDNI.Text = "";
            TIngresoClave.Text = "";
            this.Show();
        }
    }
}
