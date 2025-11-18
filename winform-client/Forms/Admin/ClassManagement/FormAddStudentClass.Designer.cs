namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    partial class FormAddStudentClass
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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            btnLuu = new Guna.UI2.WinForms.Guna2Button();
            btnHuy = new Guna.UI2.WinForms.Guna2Button();
            dgvDSHocSinh = new Guna.UI2.WinForms.Guna2DataGridView();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvDSHocSinh).BeginInit();
            SuspendLayout();
            // 
            // btnLuu
            // 
            btnLuu.BorderRadius = 20;
            btnLuu.CustomizableEdges = customizableEdges5;
            btnLuu.DisabledState.BorderColor = Color.DarkGray;
            btnLuu.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLuu.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLuu.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLuu.Font = new Font("Segoe UI", 9F);
            btnLuu.ForeColor = Color.White;
            btnLuu.Location = new Point(519, 414);
            btnLuu.Name = "btnLuu";
            btnLuu.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnLuu.Size = new Size(162, 47);
            btnLuu.TabIndex = 27;
            btnLuu.Text = "Lưu thay đổi";
            //btnLuu.Click += btnLuu_Click;
            // 
            // btnHuy
            // 
            btnHuy.BorderRadius = 20;
            btnHuy.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            btnHuy.CustomizableEdges = customizableEdges7;
            btnHuy.DisabledState.BorderColor = Color.DarkGray;
            btnHuy.DisabledState.CustomBorderColor = Color.DarkGray;
            btnHuy.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnHuy.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnHuy.FillColor = Color.Silver;
            btnHuy.Font = new Font("Segoe UI", 9F);
            btnHuy.ForeColor = Color.White;
            btnHuy.Location = new Point(387, 414);
            btnHuy.Name = "btnHuy";
            btnHuy.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnHuy.Size = new Size(105, 47);
            btnHuy.TabIndex = 26;
            btnHuy.Text = "Hủy";
            //btnHuy.Click += btnHuy_Click;
            // 
            // dgvDSHocSinh
            // 
            dataGridViewCellStyle4.BackColor = Color.White;
            dgvDSHocSinh.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvDSHocSinh.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvDSHocSinh.ColumnHeadersHeight = 4;
            dgvDSHocSinh.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvDSHocSinh.DefaultCellStyle = dataGridViewCellStyle6;
            dgvDSHocSinh.GridColor = Color.FromArgb(231, 229, 255);
            dgvDSHocSinh.Location = new Point(54, 71);
            dgvDSHocSinh.Name = "dgvDSHocSinh";
            dgvDSHocSinh.RowHeadersVisible = false;
            dgvDSHocSinh.RowHeadersWidth = 51;
            dgvDSHocSinh.Size = new Size(626, 304);
            dgvDSHocSinh.TabIndex = 25;
            dgvDSHocSinh.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvDSHocSinh.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvDSHocSinh.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvDSHocSinh.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvDSHocSinh.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvDSHocSinh.ThemeStyle.BackColor = Color.White;
            dgvDSHocSinh.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvDSHocSinh.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvDSHocSinh.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvDSHocSinh.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvDSHocSinh.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvDSHocSinh.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvDSHocSinh.ThemeStyle.HeaderStyle.Height = 4;
            dgvDSHocSinh.ThemeStyle.ReadOnly = false;
            dgvDSHocSinh.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvDSHocSinh.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDSHocSinh.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvDSHocSinh.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvDSHocSinh.ThemeStyle.RowsStyle.Height = 29;
            dgvDSHocSinh.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvDSHocSinh.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(54, 23);
            label1.Name = "label1";
            label1.Size = new Size(222, 28);
            label1.TabIndex = 24;
            label1.Text = "Chọn học sinh cho lớp";
            // 
            // FormAddStudentClass
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(737, 504);
            Controls.Add(btnLuu);
            Controls.Add(btnHuy);
            Controls.Add(dgvDSHocSinh);
            Controls.Add(label1);
            Name = "FormAddStudentClass";
            Text = "FormAddStudentClass";
            ((System.ComponentModel.ISupportInitialize)dgvDSHocSinh).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnLuu;
        private Guna.UI2.WinForms.Guna2Button btnHuy;
        private Guna.UI2.WinForms.Guna2DataGridView dgvDSHocSinh;
        private Label label1;
    }
}