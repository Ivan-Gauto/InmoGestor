using System.Windows.Forms;

namespace InmoGestor
{
    partial class Usuarios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label13;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Usuarios));
            this.ContenedorUsuarios = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BAgregarUsuario = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.LCantUsuarios = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridUsuarios = new System.Windows.Forms.DataGridView();
            this.ColumnaAcciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaRol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaCorreo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaTelefono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaDireccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaApellido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnaNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsuarioColumna = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label2 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            this.ContenedorUsuarios.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridUsuarios)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContenedorUsuarios
            // 
            this.ContenedorUsuarios.Controls.Add(this.tableLayoutPanel1);
            this.ContenedorUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContenedorUsuarios.Location = new System.Drawing.Point(0, 0);
            this.ContenedorUsuarios.Name = "ContenedorUsuarios";
            this.ContenedorUsuarios.Size = new System.Drawing.Size(799, 467);
            this.ContenedorUsuarios.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.BAgregarUsuario, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(30, 30);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(739, 61);
            this.tableLayoutPanel3.TabIndex = 20;
            // 
            // BAgregarUsuario
            // 
            this.BAgregarUsuario.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BAgregarUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(35)))));
            this.BAgregarUsuario.FlatAppearance.BorderSize = 0;
            this.BAgregarUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BAgregarUsuario.ForeColor = System.Drawing.Color.White;
            this.BAgregarUsuario.Location = new System.Drawing.Point(573, 4);
            this.BAgregarUsuario.Name = "BAgregarUsuario";
            this.BAgregarUsuario.Size = new System.Drawing.Size(163, 53);
            this.BAgregarUsuario.TabIndex = 23;
            this.BAgregarUsuario.Text = "Agregar usuario";
            this.BAgregarUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BAgregarUsuario.UseVisualStyleBackColor = false;
            this.BAgregarUsuario.Click += new System.EventHandler(this.BAgregarUsuario_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(label2, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(244, 55);
            this.tableLayoutPanel5.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(0, 1);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 37);
            this.label12.TabIndex = 19;
            this.label12.Text = "Usuarios";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(35)))));
            label2.Dock = System.Windows.Forms.DockStyle.Left;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label2.Location = new System.Drawing.Point(5, 40);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(221, 15);
            label2.TabIndex = 41;
            label2.Text = "Gestion de usuarios y acceso al sistema";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.62311F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.12563F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.12563F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.12563F));
            this.tableLayoutPanel2.Controls.Add(this.panel7, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel4, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(30, 121);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(739, 77);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(label9);
            this.panel2.Controls.Add(this.LCantUsuarios);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 0, 40, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(141, 77);
            this.panel2.TabIndex = 19;
            // 
            // LCantUsuarios
            // 
            this.LCantUsuarios.AutoSize = true;
            this.LCantUsuarios.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LCantUsuarios.ForeColor = System.Drawing.Color.White;
            this.LCantUsuarios.Location = new System.Drawing.Point(16, 27);
            this.LCantUsuarios.Name = "LCantUsuarios";
            this.LCantUsuarios.Size = new System.Drawing.Size(28, 32);
            this.LCantUsuarios.TabIndex = 3;
            this.LCantUsuarios.Text = "0";
            // 
            // label9
            // 
            label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label9.Location = new System.Drawing.Point(10, 59);
            label9.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(117, 17);
            label9.TabIndex = 11;
            label9.Text = "Total de inmuebles";
            label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel6.Controls.Add(label10);
            this.panel6.Controls.Add(this.label11);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(141, 77);
            this.panel6.TabIndex = 24;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(16, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 32);
            this.label11.TabIndex = 15;
            this.label11.Text = "0";
            // 
            // label10
            // 
            label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label10.Location = new System.Drawing.Point(10, 42);
            label10.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(117, 17);
            label10.TabIndex = 16;
            label10.Text = "Total de usuarios";
            label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(181, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 40, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(145, 77);
            this.panel1.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(16, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 32);
            this.label4.TabIndex = 3;
            this.label4.Text = "0";
            // 
            // label3
            // 
            label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label3.Location = new System.Drawing.Point(10, 59);
            label3.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(117, 17);
            label3.TabIndex = 11;
            label3.Text = "Contratos activos";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel3.Controls.Add(label7);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(145, 77);
            this.panel3.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(16, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 32);
            this.label8.TabIndex = 15;
            this.label8.Text = "0";
            // 
            // label7
            // 
            label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label7.Location = new System.Drawing.Point(10, 42);
            label7.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(117, 17);
            label7.TabIndex = 16;
            label7.Text = "Administradores";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel4.Controls.Add(label5);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(366, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0, 0, 35, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(150, 77);
            this.panel4.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(16, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 32);
            this.label6.TabIndex = 15;
            this.label6.Text = "0";
            // 
            // label5
            // 
            label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label5.Location = new System.Drawing.Point(19, 42);
            label5.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(81, 17);
            label5.TabIndex = 16;
            label5.Text = "Inmobiliarios";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel7.Controls.Add(label13);
            this.panel7.Controls.Add(this.label14);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(551, 0);
            this.panel7.Margin = new System.Windows.Forms.Padding(0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(188, 77);
            this.panel7.TabIndex = 24;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(16, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 32);
            this.label14.TabIndex = 15;
            this.label14.Text = "0";
            // 
            // label13
            // 
            label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            label13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(183)))), ((int)(((byte)(217)))));
            label13.Location = new System.Drawing.Point(19, 42);
            label13.Margin = new System.Windows.Forms.Padding(0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(69, 17);
            label13.TabIndex = 16;
            label13.Text = "Ayudantes";
            label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(30, 198);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(739, 239);
            this.tableLayoutPanel4.TabIndex = 19;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.panel5.Controls.Add(this.dataGridUsuarios);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 40);
            this.panel5.Margin = new System.Windows.Forms.Padding(0, 40, 0, 67);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(24, 72, 24, 25);
            this.panel5.Size = new System.Drawing.Size(739, 132);
            this.panel5.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(19, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 30);
            this.label1.TabIndex = 14;
            this.label1.Text = "Usuarios";
            // 
            // dataGridUsuarios
            // 
            this.dataGridUsuarios.AllowUserToAddRows = false;
            this.dataGridUsuarios.AllowUserToOrderColumns = true;
            this.dataGridUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridUsuarios.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.dataGridUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridUsuarios.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridUsuarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UsuarioColumna,
            this.ColumnaNombre,
            this.ColumnaApellido,
            this.ColumnaDireccion,
            this.ColumnaTelefono,
            this.ColumnaCorreo,
            this.ColumnaRol,
            this.ColumnaEstado,
            this.ColumnaAcciones});
            this.dataGridUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridUsuarios.EnableHeadersVisualStyles = false;
            this.dataGridUsuarios.GridColor = System.Drawing.Color.Teal;
            this.dataGridUsuarios.Location = new System.Drawing.Point(24, 72);
            this.dataGridUsuarios.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridUsuarios.Name = "dataGridUsuarios";
            this.dataGridUsuarios.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridUsuarios.RowHeadersVisible = false;
            this.dataGridUsuarios.RowHeadersWidth = 62;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridUsuarios.RowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridUsuarios.Size = new System.Drawing.Size(691, 35);
            this.dataGridUsuarios.TabIndex = 16;
            // 
            // ColumnaAcciones
            // 
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaAcciones.DefaultCellStyle = dataGridViewCellStyle10;
            this.ColumnaAcciones.HeaderText = "Acciones";
            this.ColumnaAcciones.MinimumWidth = 8;
            this.ColumnaAcciones.Name = "ColumnaAcciones";
            // 
            // ColumnaEstado
            // 
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaEstado.DefaultCellStyle = dataGridViewCellStyle9;
            this.ColumnaEstado.HeaderText = "Estado";
            this.ColumnaEstado.MinimumWidth = 8;
            this.ColumnaEstado.Name = "ColumnaEstado";
            // 
            // ColumnaRol
            // 
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaRol.DefaultCellStyle = dataGridViewCellStyle8;
            this.ColumnaRol.HeaderText = "Rol";
            this.ColumnaRol.MinimumWidth = 8;
            this.ColumnaRol.Name = "ColumnaRol";
            // 
            // ColumnaCorreo
            // 
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaCorreo.DefaultCellStyle = dataGridViewCellStyle7;
            this.ColumnaCorreo.HeaderText = "Correo";
            this.ColumnaCorreo.MinimumWidth = 8;
            this.ColumnaCorreo.Name = "ColumnaCorreo";
            // 
            // ColumnaTelefono
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaTelefono.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColumnaTelefono.HeaderText = "Telefono";
            this.ColumnaTelefono.MinimumWidth = 8;
            this.ColumnaTelefono.Name = "ColumnaTelefono";
            // 
            // ColumnaDireccion
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaDireccion.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnaDireccion.HeaderText = "Direccion";
            this.ColumnaDireccion.MinimumWidth = 8;
            this.ColumnaDireccion.Name = "ColumnaDireccion";
            // 
            // ColumnaApellido
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaApellido.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnaApellido.HeaderText = "Apellido";
            this.ColumnaApellido.MinimumWidth = 8;
            this.ColumnaApellido.Name = "ColumnaApellido";
            // 
            // ColumnaNombre
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.ColumnaNombre.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnaNombre.HeaderText = "Nombre";
            this.ColumnaNombre.MinimumWidth = 8;
            this.ColumnaNombre.Name = "ColumnaNombre";
            // 
            // UsuarioColumna
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(123)))), ((int)(((byte)(185)))));
            this.UsuarioColumna.DefaultCellStyle = dataGridViewCellStyle2;
            this.UsuarioColumna.HeaderText = "Usuario";
            this.UsuarioColumna.MinimumWidth = 8;
            this.UsuarioColumna.Name = "UsuarioColumna";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(35)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(799, 467);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 61F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 107F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 238F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(799, 467);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // Usuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScrollMinSize = new System.Drawing.Size(799, 467);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(799, 467);
            this.Controls.Add(this.ContenedorUsuarios);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Usuarios";
            this.Text = "Usuarios";
            this.Load += new System.EventHandler(this.Usuarios_Load);
            this.ContenedorUsuarios.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridUsuarios)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel ContenedorUsuarios;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel panel5;
        private DataGridView dataGridUsuarios;
        private DataGridViewTextBoxColumn UsuarioColumna;
        private DataGridViewTextBoxColumn ColumnaNombre;
        private DataGridViewTextBoxColumn ColumnaApellido;
        private DataGridViewTextBoxColumn ColumnaDireccion;
        private DataGridViewTextBoxColumn ColumnaTelefono;
        private DataGridViewTextBoxColumn ColumnaCorreo;
        private DataGridViewTextBoxColumn ColumnaRol;
        private DataGridViewTextBoxColumn ColumnaEstado;
        private DataGridViewTextBoxColumn ColumnaAcciones;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel7;
        private Label label14;
        private Panel panel4;
        private Label label6;
        private Panel panel1;
        private Panel panel3;
        private Label label8;
        private Label label4;
        private Panel panel2;
        private Panel panel6;
        private Label label11;
        private Label LCantUsuarios;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label12;
        private Button BAgregarUsuario;
    }
}