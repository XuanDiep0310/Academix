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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle11 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle12 = new DataGridViewCellStyle();
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
            lblClassCode.Location = new Point(27, 27);
            lblClassCode.Name = "lblClassCode";
            lblClassCode.Size = new Size(121, 22);
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
            guna2HtmlLabel3.Location = new Point(31, 114);
            guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            guna2HtmlLabel3.Size = new Size(65, 22);
            guna2HtmlLabel3.TabIndex = 2;
            guna2HtmlLabel3.Text = "Giáo viên";
            // 
            // guna2HtmlLabel4
            // 
            guna2HtmlLabel4.BackColor = Color.Transparent;
            guna2HtmlLabel4.Location = new Point(27, 345);
            guna2HtmlLabel4.Name = "guna2HtmlLabel4";
            guna2HtmlLabel4.Size = new Size(60, 22);
            guna2HtmlLabel4.TabIndex = 3;
            guna2HtmlLabel4.Text = "Học sinh";
            // 
            // btnThemGiaoVien
            // 
            btnThemGiaoVien.BorderRadius = 20;
            btnThemGiaoVien.CustomizableEdges = customizableEdges5;
            btnThemGiaoVien.DisabledState.BorderColor = Color.DarkGray;
            btnThemGiaoVien.DisabledState.CustomBorderColor = Color.DarkGray;
            btnThemGiaoVien.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnThemGiaoVien.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnThemGiaoVien.Font = new Font("Segoe UI", 9F);
            btnThemGiaoVien.ForeColor = Color.White;
            btnThemGiaoVien.Location = new Point(496, 105);
            btnThemGiaoVien.Name = "btnThemGiaoVien";
            btnThemGiaoVien.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnThemGiaoVien.Size = new Size(164, 47);
            btnThemGiaoVien.TabIndex = 4;
            btnThemGiaoVien.Text = "Thêm giáo viên";
            btnThemGiaoVien.Click += btnThemGiaoVien_Click;
            // 
            // btnThemHocSinh
            // 
            btnThemHocSinh.BorderRadius = 20;
            btnThemHocSinh.CustomizableEdges = customizableEdges7;
            btnThemHocSinh.DisabledState.BorderColor = Color.DarkGray;
            btnThemHocSinh.DisabledState.CustomBorderColor = Color.DarkGray;
            btnThemHocSinh.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnThemHocSinh.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnThemHocSinh.Font = new Font("Segoe UI", 9F);
            btnThemHocSinh.ForeColor = Color.White;
            btnThemHocSinh.Location = new Point(485, 345);
            btnThemHocSinh.Name = "btnThemHocSinh";
            btnThemHocSinh.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnThemHocSinh.Size = new Size(164, 47);
            btnThemHocSinh.TabIndex = 5;
            btnThemHocSinh.Text = "Thêm học sinh";
            btnThemHocSinh.Click += btnThemHocSinh_Click;
            // 
            // dgvGiaoVien
            // 
            dataGridViewCellStyle7.BackColor = Color.White;
            dgvGiaoVien.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle8.ForeColor = Color.White;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            dgvGiaoVien.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            dgvGiaoVien.ColumnHeadersHeight = 4;
            dgvGiaoVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = Color.White;
            dataGridViewCellStyle9.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle9.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle9.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle9.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.False;
            dgvGiaoVien.DefaultCellStyle = dataGridViewCellStyle9;
            dgvGiaoVien.GridColor = Color.FromArgb(231, 229, 255);
            dgvGiaoVien.Location = new Point(31, 158);
            dgvGiaoVien.Name = "dgvGiaoVien";
            dgvGiaoVien.RowHeadersVisible = false;
            dgvGiaoVien.RowHeadersWidth = 51;
            dgvGiaoVien.Size = new Size(633, 147);
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
            dgvGiaoVien.ThemeStyle.HeaderStyle.Height = 4;
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
            dataGridViewCellStyle10.BackColor = Color.White;
            dgvHocSinh.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle11.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle11.ForeColor = Color.White;
            dataGridViewCellStyle11.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = DataGridViewTriState.True;
            dgvHocSinh.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            dgvHocSinh.ColumnHeadersHeight = 4;
            dgvHocSinh.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = Color.White;
            dataGridViewCellStyle12.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle12.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle12.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle12.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle12.WrapMode = DataGridViewTriState.False;
            dgvHocSinh.DefaultCellStyle = dataGridViewCellStyle12;
            dgvHocSinh.GridColor = Color.FromArgb(231, 229, 255);
            dgvHocSinh.Location = new Point(27, 398);
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
            dgvHocSinh.ThemeStyle.HeaderStyle.Height = 4;
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
            ClientSize = new Size(692, 658);
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