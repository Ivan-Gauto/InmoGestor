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
    public partial class AgregarContrato : Form
    {
        public AgregarContrato()
        {
            InitializeComponent();
        }

        private void BCerrarForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
