namespace InmoGestor
{
    partial class DashboardForm
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
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            panelCards = new Panel();
            cardInquilinos = new GroupBox();
            lblInquilinos = new Label();
            cardContratos = new GroupBox();
            lblContratos = new Label();
            cardIngresos = new GroupBox();
            label1 = new Label();
            lblIngresos = new Label();
            cardInmuebles = new GroupBox();
            lblInmuebles = new Label();
            panelPagos = new GroupBox();
            panelContratosVencer = new GroupBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            panelHeader.SuspendLayout();
            panelCards.SuspendLayout();
            cardInquilinos.SuspendLayout();
            cardContratos.SuspendLayout();
            cardIngresos.SuspendLayout();
            cardInmuebles.SuspendLayout();
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
            panelHeader.Size = new Size(1478, 74);
            panelHeader.TabIndex = 1;
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.LightGray;
            lblSubtitulo.Location = new Point(450, 20);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(100, 23);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Resumen general del sistema";
            lblSubtitulo.Click += lblSubtitulo_Click;
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(193, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(288, 51);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Dashboard";
            // 
            // panelCards
            // 
            panelCards.Controls.Add(cardInquilinos);
            panelCards.Controls.Add(cardContratos);
            panelCards.Controls.Add(cardIngresos);
            panelCards.Location = new Point(220, 80);
            panelCards.Name = "panelCards";
            panelCards.Size = new Size(900, 120);
            panelCards.TabIndex = 2;
            // 
            // cardInquilinos
            // 
            cardInquilinos.Controls.Add(lblInquilinos);
            cardInquilinos.ForeColor = SystemColors.ButtonFace;
            cardInquilinos.Location = new Point(210, 0);
            cardInquilinos.Name = "cardInquilinos";
            cardInquilinos.Size = new Size(200, 100);
            cardInquilinos.TabIndex = 1;
            cardInquilinos.TabStop = false;
            cardInquilinos.Text = "Inquilinos Activos";
            // 
            // lblInquilinos
            // 
            lblInquilinos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblInquilinos.ForeColor = Color.White;
            lblInquilinos.Location = new Point(20, 40);
            lblInquilinos.Name = "lblInquilinos";
            lblInquilinos.Size = new Size(109, 42);
            lblInquilinos.TabIndex = 0;
            lblInquilinos.Text = "98";
            // 
            // cardContratos
            // 
            cardContratos.Controls.Add(lblContratos);
            cardContratos.ForeColor = SystemColors.ButtonFace;
            cardContratos.Location = new Point(420, 0);
            cardContratos.Name = "cardContratos";
            cardContratos.Size = new Size(200, 100);
            cardContratos.TabIndex = 2;
            cardContratos.TabStop = false;
            cardContratos.Text = "Contratos Activos";
            cardContratos.Enter += cardContratos_Enter;
            // 
            // lblContratos
            // 
            lblContratos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblContratos.ForeColor = Color.White;
            lblContratos.Location = new Point(17, 40);
            lblContratos.Name = "lblContratos";
            lblContratos.Size = new Size(105, 42);
            lblContratos.TabIndex = 0;
            lblContratos.Text = "87";
            // 
            // cardIngresos
            // 
            cardIngresos.Controls.Add(label1);
            cardIngresos.Controls.Add(lblIngresos);
            cardIngresos.ForeColor = SystemColors.ButtonFace;
            cardIngresos.Location = new Point(630, 0);
            cardIngresos.Name = "cardIngresos";
            cardIngresos.Size = new Size(200, 100);
            cardIngresos.TabIndex = 3;
            cardIngresos.TabStop = false;
            cardIngresos.Enter += cardIngresos_Enter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 20);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 1;
            label1.Text = "label1";
            label1.Click += label1_Click;
            // 
            // lblIngresos
            // 
            lblIngresos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblIngresos.ForeColor = Color.White;
            lblIngresos.Location = new Point(20, 40);
            lblIngresos.Name = "lblIngresos";
            lblIngresos.Size = new Size(160, 42);
            lblIngresos.TabIndex = 0;
            lblIngresos.Text = "$245.000";
            // 
            // cardInmuebles
            // 
            cardInmuebles.Controls.Add(lblInmuebles);
            cardInmuebles.ForeColor = SystemColors.ButtonFace;
            cardInmuebles.Location = new Point(220, 83);
            cardInmuebles.Name = "cardInmuebles";
            cardInmuebles.Size = new Size(200, 100);
            cardInmuebles.TabIndex = 0;
            cardInmuebles.TabStop = false;
            cardInmuebles.Text = "Inmuebles Totales";
            cardInmuebles.Enter += cardInmuebles_Enter;
            // 
            // lblInmuebles
            // 
            lblInmuebles.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblInmuebles.ForeColor = Color.White;
            lblInmuebles.Location = new Point(20, 40);
            lblInmuebles.Name = "lblInmuebles";
            lblInmuebles.Size = new Size(110, 42);
            lblInmuebles.TabIndex = 0;
            lblInmuebles.Text = "124";
            // 
            // panelPagos
            // 
            panelPagos.BackColor = Color.FromArgb(20, 40, 60);
            panelPagos.Location = new Point(220, 220);
            panelPagos.Name = "panelPagos";
            panelPagos.Size = new Size(400, 200);
            panelPagos.TabIndex = 3;
            panelPagos.TabStop = false;
            panelPagos.Text = "Pagos Recientes";
            // 
            // panelContratosVencer
            // 
            panelContratosVencer.BackColor = Color.FromArgb(20, 40, 60);
            panelContratosVencer.Location = new Point(630, 220);
            panelContratosVencer.Name = "panelContratosVencer";
            panelContratosVencer.Size = new Size(400, 200);
            panelContratosVencer.TabIndex = 4;
            panelContratosVencer.TabStop = false;
            panelContratosVencer.Text = "Contratos por Vencer";
            panelContratosVencer.Enter += panelContratosVencer_Enter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(963, 471);
            label2.Name = "label2";
            label2.Size = new Size(59, 25);
            label2.TabIndex = 2;
            label2.Text = "label2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(728, 450);
            label3.Name = "label3";
            label3.Size = new Size(59, 25);
            label3.TabIndex = 3;
            label3.Text = "label3";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(841, 450);
            label4.Name = "label4";
            label4.Size = new Size(59, 25);
            label4.TabIndex = 4;
            label4.Text = "label4";
            label4.Click += label4_Click;
            // 
            // DashboardForm
            // 
            BackColor = Color.FromArgb(10, 20, 35);
            ClientSize = new Size(1478, 784);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(cardInmuebles);
            Controls.Add(panelSidebar);
            Controls.Add(panelHeader);
            Controls.Add(panelCards);
            Controls.Add(panelPagos);
            Controls.Add(panelContratosVencer);
            Name = "DashboardForm";
            Text = "Dashboard - InmoGestor";
            Load += DashboardForm_Load;
            panelHeader.ResumeLayout(false);
            panelCards.ResumeLayout(false);
            cardInquilinos.ResumeLayout(false);
            cardContratos.ResumeLayout(false);
            cardIngresos.ResumeLayout(false);
            cardIngresos.PerformLayout();
            cardInmuebles.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel panelCards;
        private System.Windows.Forms.GroupBox cardInmuebles;
        private System.Windows.Forms.Label lblInmuebles;
        private System.Windows.Forms.GroupBox cardInquilinos;
        private System.Windows.Forms.Label lblInquilinos;
        private System.Windows.Forms.GroupBox cardContratos;
        private System.Windows.Forms.Label lblContratos;
        private System.Windows.Forms.GroupBox cardIngresos;
        private System.Windows.Forms.Label lblIngresos;
        private System.Windows.Forms.GroupBox panelPagos;
        private System.Windows.Forms.GroupBox panelContratosVencer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
    }
}