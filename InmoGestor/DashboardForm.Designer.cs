// DashboardForm.Designer.cs
namespace InmoGestor
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelSidebar = new Panel();
            panelHeader = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            panelCards = new Panel();
            cardIngresos = new GroupBox();
            lblIngresos = new Label();
            cardContratos = new GroupBox();
            lblContratos = new Label();
            cardInquilinos = new GroupBox();
            lblInquilinos = new Label();
            cardInmuebles = new GroupBox();
            lblInmuebles = new Label();
            panelPagos = new GroupBox();
            panelContratosVencer = new GroupBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            tlpContent = new TableLayoutPanel();
            panelHeader.SuspendLayout();
            panelCards.SuspendLayout();
            cardIngresos.SuspendLayout();
            cardContratos.SuspendLayout();
            cardInquilinos.SuspendLayout();
            cardInmuebles.SuspendLayout();
            tlpContent.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.BackColor = Color.FromArgb(20, 40, 60);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 72);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(220, 728);
            panelSidebar.TabIndex = 2;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(15, 30, 45);
            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1280, 72);
            panelHeader.TabIndex = 3;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(200, 15);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(193, 46);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Dashboard";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.LightGray;
            lblSubtitulo.Location = new Point(500, 26);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(232, 23);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Resumen general del sistema";
            // 
            // panelCards
            // 
            panelCards.BackColor = Color.Transparent;
            panelCards.Controls.Add(cardIngresos);
            panelCards.Controls.Add(cardContratos);
            panelCards.Controls.Add(cardInquilinos);
            panelCards.Controls.Add(cardInmuebles);
            panelCards.Dock = DockStyle.Top;
            panelCards.Location = new Point(220, 72);
            panelCards.Name = "panelCards";
            panelCards.Padding = new Padding(16, 8, 16, 8);
            panelCards.Size = new Size(1060, 120);
            panelCards.TabIndex = 1;
            // 
            // cardIngresos
            // 
            cardIngresos.Controls.Add(lblIngresos);
            cardIngresos.ForeColor = SystemColors.ButtonFace;
            cardIngresos.Location = new Point(630, 10);
            cardIngresos.Name = "cardIngresos";
            cardIngresos.Size = new Size(200, 100);
            cardIngresos.TabIndex = 0;
            cardIngresos.TabStop = false;
            cardIngresos.Text = "Ingresos";
            // 
            // lblIngresos
            // 
            lblIngresos.AutoSize = true;
            lblIngresos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblIngresos.ForeColor = Color.White;
            lblIngresos.Location = new Point(20, 40);
            lblIngresos.Name = "lblIngresos";
            lblIngresos.Size = new Size(145, 41);
            lblIngresos.TabIndex = 0;
            lblIngresos.Text = "$245.000";
            // 
            // cardContratos
            // 
            cardContratos.Controls.Add(lblContratos);
            cardContratos.ForeColor = SystemColors.ButtonFace;
            cardContratos.Location = new Point(420, 10);
            cardContratos.Name = "cardContratos";
            cardContratos.Size = new Size(200, 100);
            cardContratos.TabIndex = 1;
            cardContratos.TabStop = false;
            cardContratos.Text = "Contratos Activos";
            // 
            // lblContratos
            // 
            lblContratos.AutoSize = true;
            lblContratos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblContratos.ForeColor = Color.White;
            lblContratos.Location = new Point(20, 40);
            lblContratos.Name = "lblContratos";
            lblContratos.Size = new Size(52, 41);
            lblContratos.TabIndex = 0;
            lblContratos.Text = "87";
            // 
            // cardInquilinos
            // 
            cardInquilinos.Controls.Add(lblInquilinos);
            cardInquilinos.ForeColor = SystemColors.ButtonFace;
            cardInquilinos.Location = new Point(210, 10);
            cardInquilinos.Name = "cardInquilinos";
            cardInquilinos.Size = new Size(200, 100);
            cardInquilinos.TabIndex = 2;
            cardInquilinos.TabStop = false;
            cardInquilinos.Text = "Inquilinos Activos";
            // 
            // lblInquilinos
            // 
            lblInquilinos.AutoSize = true;
            lblInquilinos.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblInquilinos.ForeColor = Color.White;
            lblInquilinos.Location = new Point(20, 40);
            lblInquilinos.Name = "lblInquilinos";
            lblInquilinos.Size = new Size(52, 41);
            lblInquilinos.TabIndex = 0;
            lblInquilinos.Text = "98";
            // 
            // cardInmuebles
            // 
            cardInmuebles.Controls.Add(lblInmuebles);
            cardInmuebles.ForeColor = SystemColors.ButtonFace;
            cardInmuebles.Location = new Point(0, 10);
            cardInmuebles.Name = "cardInmuebles";
            cardInmuebles.Size = new Size(200, 100);
            cardInmuebles.TabIndex = 3;
            cardInmuebles.TabStop = false;
            cardInmuebles.Text = "Inmuebles Totales";
            // 
            // lblInmuebles
            // 
            lblInmuebles.AutoSize = true;
            lblInmuebles.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblInmuebles.ForeColor = Color.White;
            lblInmuebles.Location = new Point(20, 40);
            lblInmuebles.Name = "lblInmuebles";
            lblInmuebles.Size = new Size(69, 41);
            lblInmuebles.TabIndex = 0;
            lblInmuebles.Text = "124";
            // 
            // panelPagos
            // 
            panelPagos.BackColor = Color.FromArgb(20, 40, 60);
            panelPagos.Dock = DockStyle.Fill;
            panelPagos.Location = new Point(19, 11);
            panelPagos.Name = "panelPagos";
            panelPagos.Size = new Size(508, 578);
            panelPagos.TabIndex = 0;
            panelPagos.TabStop = false;
            panelPagos.Text = "Pagos Recientes";
            // 
            // panelContratosVencer
            // 
            panelContratosVencer.BackColor = Color.FromArgb(20, 40, 60);
            panelContratosVencer.Dock = DockStyle.Fill;
            panelContratosVencer.Location = new Point(533, 11);
            panelContratosVencer.Name = "panelContratosVencer";
            panelContratosVencer.Size = new Size(508, 578);
            panelContratosVencer.TabIndex = 1;
            panelContratosVencer.TabStop = false;
            panelContratosVencer.Text = "Contratos por Vencer";
            // 
            // tlpContent
            // 
            tlpContent.ColumnCount = 2;
            tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpContent.Controls.Add(panelPagos, 0, 0);
            tlpContent.Controls.Add(panelContratosVencer, 1, 0);
            tlpContent.Dock = DockStyle.Fill;
            tlpContent.Location = new Point(220, 192);
            tlpContent.Name = "tlpContent";
            tlpContent.Padding = new Padding(16, 8, 16, 16);
            tlpContent.RowCount = 1;
            tlpContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpContent.Size = new Size(1060, 608);
            tlpContent.TabIndex = 0;
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(10, 20, 35);
            ClientSize = new Size(1280, 800);
            Controls.Add(tlpContent);
            Controls.Add(panelCards);
            Controls.Add(panelSidebar);
            Controls.Add(panelHeader);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(1100, 700);
            Name = "DashboardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard - InmoGestor";
            Load += DashboardForm_Load_1;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelCards.ResumeLayout(false);
            cardIngresos.ResumeLayout(false);
            cardIngresos.PerformLayout();
            cardContratos.ResumeLayout(false);
            cardContratos.PerformLayout();
            cardInquilinos.ResumeLayout(false);
            cardInquilinos.PerformLayout();
            cardInmuebles.ResumeLayout(false);
            cardInmuebles.PerformLayout();
            tlpContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // ----- CAMPOS (deben existir para que no marque "no existe en el contexto") -----
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
        private TableLayoutPanel tlpContent;
    }
}
