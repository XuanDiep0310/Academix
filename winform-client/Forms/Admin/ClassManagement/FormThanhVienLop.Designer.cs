namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    partial class FormThanhVienLop
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            lblClassCode = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel4 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnThemGiaoVien = new Guna.UI2.WinForms.Guna2Button();
            btnThemHocSinh = new Guna.UI2.WinForms.Guna2Button();
            dgvGiaoVien = new Guna.UI2.WinForms.Guna2DataGridView();
            dgvHocSinh = new Guna.UI2.WinForms.Guna2DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvGiaoVien).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvHocSinh).BeginInit();
            SuspendLayout();
            // 
            // lblClassCode
            // 
            lblClassCode.BackColor = Color.Transparent;
            lblClassCode.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClassCode.Location = new Point(27, 27);
            lblClassCode.Name = "lblClassCode";
            lblClassCode.Size = new Size(171, 30);
            lblClassCode.TabIndex = 0;
            lblClassCode.Text = "guna2HtmlLabel1";
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.ForeColor = SystemColors.ButtonShadow;
            guna2HtmlLabel2.Location = new Point(27, 66);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(261, 22);
            guna2HtmlLabel2.TabIndex = 1;
            guna2HtmlLabel2.Text = "Quản lý giáo viên và học sinh trong lớp";
            // 
            // guna2HtmlLabel3
            // 
            guna2HtmlLabel3.BackColor = Color.Transparent;
            guna2HtmlLabel3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel3.Location = new Point(31, 114);
            guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            guna2HtmlLabel3.Size = new Size(77, 25);
            guna2HtmlLabel3.TabIndex = 2;
            guna2HtmlLabel3.Text = "Giáo viên";
            // 
            // guna2HtmlLabel4
            // 
            guna2HtmlLabel4.BackColor = Color.Transparent;
            guna2HtmlLabel4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel4.Location = new Point(27, 312);
            guna2HtmlLabel4.Name = "guna2HtmlLabel4";
            guna2HtmlLabel4.Size = new Size(71, 25);
            guna2HtmlLabel4.TabIndex = 3;
            guna2HtmlLabel4.Text = "Học sinh";
            // 
            // btnThemGiaoVien
            // 
            btnThemGiaoVien.BorderRadius = 20;
            btnThemGiaoVien.CustomizableEdges = customizableEdges1;
            btnThemGiaoVien.DisabledState.BorderColor = Color.DarkGray;
            btnThemGiaoVien.DisabledState.CustomBorderColor = Color.DarkGray;
            btnThemGiaoVien.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnThemGiaoVien.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnThemGiaoVien.Font = new Font("Segoe UI", 9F);
            btnThemGiaoVien.ForeColor = Color.White;
            btnThemGiaoVien.Location = new Point(511, 98);
            btnThemGiaoVien.Name = "btnThemGiaoVien";
            btnThemGiaoVien.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnThemGiaoVien.Size = new Size(153, 38);
            btnThemGiaoVien.TabIndex = 4;
            btnThemGiaoVien.Text = "Thêm giáo viên";
            btnThemGiaoVien.Click += btnThemGiaoVien_Click;
            // 
            // btnThemHocSinh
            // 
            btnThemHocSinh.BorderRadius = 20;
            btnThemHocSinh.CustomizableEdges = customizableEdges3;
            btnThemHocSinh.DisabledState.BorderColor = Color.DarkGray;
            btnThemHocSinh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnThemHocSinh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnThemHocSinh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnThemHocSinh.Font = new Font("Segoe UI", 9F);
            btnThemHocSinh.ForeColor = Color.White;
            btnThemHocSinh.Location = new Point(511, 312);
            btnThemHocSinh.Name = "btnThemHocSinh";
            btnThemHocSinh.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnThemHocSinh.Size = new Size(153, 38);
            btnThemHocSinh.TabIndex = 5;
            btnThemHocSinh.Text = "Thêm học sinh";
            btnThemHocSinh.Click += btnThemHocSinh_Click;
            // 
            // dgvGiaoVien
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvGiaoVien.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvGiaoVien.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvGiaoVien.ColumnHeadersHeight = 40;
            dgvGiaoVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvGiaoVien.DefaultCellStyle = dataGridViewCellStyle3;
            dgvGiaoVien.GridColor = Color.FromArgb(231, 229, 255);
            dgvGiaoVien.Location = new Point(31, 158);
            dgvGiaoVien.Name = "dgvGiaoVien";
            dgvGiaoVien.RowHeadersVisible = false;
            dgvGiaoVien.RowHeadersWidth = 51;
            dgvGiaoVien.Size = new Size(633, 117);
            dgvGiaoVien.TabIndex = 6;
            dgvGiaoVien.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvGiaoVien.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvGiaoVien.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvGiaoVien.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvGiaoVien.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvGiaoVien.ThemeStyle.BackColor = Color.White;
            dgvGiaoVien.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvGiaoVien.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvGiaoVien.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvGiaoVien.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvGiaoVien.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvGiaoVien.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvGiaoVien.ThemeStyle.HeaderStyle.Height = 40;
            dgvGiaoVien.ThemeStyle.ReadOnly = false;
            dgvGiaoVien.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvGiaoVien.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvGiaoVien.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvGiaoVien.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvGiaoVien.ThemeStyle.RowsStyle.Height = 29;
            dgvGiaoVien.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvGiaoVien.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // dgvHocSinh
            // 
            dataGridViewCellStyle4.BackColor = Color.White;
            dgvHocSinh.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvHocSinh.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvHocSinh.ColumnHeadersHeight = 40;
            dgvHocSinh.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvHocSinh.DefaultCellStyle = dataGridViewCellStyle6;
            dgvHocSinh.GridColor = Color.FromArgb(231, 229, 255);
            dgvHocSinh.Location = new Point(27, 365);
            dgvHocSinh.Name = "dgvHocSinh";
            dgvHocSinh.RowHeadersVisible = false;
            dgvHocSinh.RowHeadersWidth = 51;
            dgvHocSinh.Size = new Size(633, 214);
            dgvHocSinh.TabIndex = 7;
            dgvHocSinh.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvHocSinh.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvHocSinh.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvHocSinh.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvHocSinh.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvHocSinh.ThemeStyle.BackColor = Color.White;
            dgvHocSinh.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvHocSinh.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvHocSinh.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvHocSinh.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvHocSinh.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvHocSinh.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvHocSinh.ThemeStyle.HeaderStyle.Height = 40;
            dgvHocSinh.ThemeStyle.ReadOnly = false;
            dgvHocSinh.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvHocSinh.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHocSinh.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvHocSinh.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvHocSinh.ThemeStyle.RowsStyle.Height = 29;
            dgvHocSinh.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvHocSinh.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // FormThanhVienLop
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(692, 644);
            Controls.Add(dgvHocSinh);
            Controls.Add(dgvGiaoVien);
            Controls.Add(btnThemHocSinh);
            Controls.Add(btnThemGiaoVien);
            Controls.Add(guna2HtmlLabel4);
            Controls.Add(guna2HtmlLabel3);
            Controls.Add(guna2HtmlLabel2);
            Controls.Add(lblClassCode);
            Name = "FormThanhVienLop";
            Text = "FormThanhVienLop";
            ((System.ComponentModel.ISupportInitialize)dgvGiaoVien).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvHocSinh).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblClassCode;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel4;
        private Guna.UI2.WinForms.Guna2Button btnThemGiaoVien;
        private Guna.UI2.WinForms.Guna2Button btnThemHocSinh;
        private Guna.UI2.WinForms.Guna2DataGridView dgvGiaoVien;
        private Guna.UI2.WinForms.Guna2DataGridView dgvHocSinh;
    }
}