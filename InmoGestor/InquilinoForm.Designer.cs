namespace InmoGestor
{
    partial class InquilinoForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelSidebar = new Panel();
            panelHeader = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            panelContenido = new Panel();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.BackColor = Color.FromArgb(20, 40, 60);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 74);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(200, 710);
            panelSidebar.TabIndex = 0;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(15, 30, 45);
            panelHeader.Controls.Add(lblSubtitulo);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(900, 74);
            panelHeader.TabIndex = 1;
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(193, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(288, 51);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Ingresar Inquilino";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.LightGray;
            lblSubtitulo.Location = new Point(450, 20);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(300, 23);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Complete los datos del nuevo inquilino";
            // 
            // panelContenido
            // 
            panelContenido.Location = new Point(220, 100);
            panelContenido.Name = "panelContenido";
            panelContenido.Size = new Size(600, 500);
            panelContenido.BackColor = Color.FromArgb(20, 40, 60);
            panelContenido.TabIndex = 2;
            // 
            // InquilinoForm
            // 
            BackColor = Color.FromArgb(10, 20, 35);
            ClientSize = new Size(900, 784);
            Controls.Add(panelSidebar);
            Controls.Add(panelHeader);
            Controls.Add(panelContenido);
            Name = "InquilinoForm";
            Text = "Ingresar Inquilino";
            panelHeader.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel panelSidebar;
        private Panel panelHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel panelContenido;
    }
}