namespace InmoGestor
{
    partial class Auxiliar
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
            System.Windows.Forms.Label label26;
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.label25 = new System.Windows.Forms.Label();
            this.panel15 = new System.Windows.Forms.Panel();
            label26 = new System.Windows.Forms.Label();
            this.tableLayoutPanel10.SuspendLayout();
            this.panel15.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 1;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.label25, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.panel15, 0, 1);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(862, 0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 2;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.81967F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.18033F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(124, 450);
            this.tableLayoutPanel10.TabIndex = 14;
            // 
            // label25
            // 
            this.label25.Dock = System.Windows.Forms.DockStyle.Right;
            this.label25.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.White;
            this.label25.Location = new System.Drawing.Point(5, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(116, 228);
            this.label25.TabIndex = 14;
            this.label25.Text = "25/08/25";
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.Color.DarkOrange;
            this.panel15.Controls.Add(label26);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(15, 233);
            this.panel15.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(94, 212);
            this.panel15.TabIndex = 15;
            // 
            // label26
            // 
            label26.BackColor = System.Drawing.Color.Transparent;
            label26.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label26.ForeColor = System.Drawing.Color.White;
            label26.Location = new System.Drawing.Point(0, 0);
            label26.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            label26.Name = "label26";
            label26.Size = new System.Drawing.Size(94, 20);
            label26.TabIndex = 18;
            label26.Text = "En 3 dias";
            label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Auxiliar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 450);
            this.Controls.Add(this.tableLayoutPanel10);
            this.Name = "Auxiliar";
            this.Text = "Auxiliar";
            this.tableLayoutPanel10.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Panel panel15;
    }
}