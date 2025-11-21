namespace Academix.WinApp.Forms.Teacher
{
    partial class UC_ClassCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblTenLop = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblMaLop = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblSoLuong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnXemDSHS = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // lblTenLop
            // 
            lblTenLop.BackColor = Color.Transparent;
            lblTenLop.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblTenLop.Location = new Point(31, 32);
            lblTenLop.Name = "lblTenLop";
            lblTenLop.Size = new Size(75, 30);
            lblTenLop.TabIndex = 0;
            lblTenLop.Text = "Tên lớp";
            // 
            // lblMaLop
            // 
            lblMaLop.BackColor = Color.Transparent;
            lblMaLop.BorderStyle = BorderStyle.FixedSingle;
            lblMaLop.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 163);
            lblMaLop.ForeColor = Color.Blue;
            lblMaLop.Location = new Point(31, 78);
            lblMaLop.Name = "lblMaLop";
            lblMaLop.Size = new Size(58, 27);
            lblMaLop.TabIndex = 1;
            lblMaLop.Text = "Mã lớp";
            // 
            // lblSoLuong
            // 
            lblSoLuong.BackColor = Color.Transparent;
            lblSoLuong.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 163);
            lblSoLuong.ForeColor = SystemColors.ControlDarkDark;
            lblSoLuong.Location = new Point(31, 155);
            lblSoLuong.Name = "lblSoLuong";
            lblSoLuong.Size = new Size(140, 25);
            lblSoLuong.TabIndex = 2;
            lblSoLuong.Text = "Số lượng học sinh";
            // 
            // btnXemDSHS
            // 
            btnXemDSHS.BorderColor = Color.DimGray;
            btnXemDSHS.BorderRadius = 15;
            btnXemDSHS.BorderThickness = 1;
            btnXemDSHS.CustomizableEdges = customizableEdges1;
            btnXemDSHS.DisabledState.BorderColor = Color.DarkGray;
            btnXemDSHS.DisabledState.CustomBorderColor = Color.DarkGray;
            btnXemDSHS.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnXemDSHS.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnXemDSHS.FillColor = Color.WhiteSmoke;
            btnXemDSHS.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btnXemDSHS.ForeColor = Color.Black;
            btnXemDSHS.Location = new Point(31, 193);
            btnXemDSHS.Name = "btnXemDSHS";
            btnXemDSHS.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnXemDSHS.Size = new Size(381, 42);
            btnXemDSHS.TabIndex = 3;
            btnXemDSHS.Text = "Xem danh sách học sinh";
            btnXemDSHS.Click += btnXemDSHS_Click;
            // 
            // UC_ClassCard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(btnXemDSHS);
            Controls.Add(lblSoLuong);
            Controls.Add(lblMaLop);
            Controls.Add(lblTenLop);
            Margin = new Padding(0);
            Name = "UC_ClassCard";
            Size = new Size(458, 252);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblTenLop;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMaLop;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoLuong;
        private Guna.UI2.WinForms.Guna2Button btnXemDSHS;
    }
}
