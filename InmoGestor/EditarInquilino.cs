using CapaEntidades;
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
    public partial class EditarInquilino : Form
    {
        public EditarInquilino(PersonaRolCliente Inquilino)
        {
            InitializeComponent();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
