namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    partial class FormAddTeacherClass
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            dgvDSGiaoVien = new Guna.UI2.WinForms.Guna2DataGridView();
            btnLuu = new Guna.UI2.WinForms.Guna2Button();
            btnHuy = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)dgvDSGiaoVien).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(34, 23);
            label1.Name = "label1";
            label1.Size = new Size(230, 28);
            label1.TabIndex = 0;
            label1.Text = "Chọn giáo viên cho lớp";
            // 
            // dgvDSGiaoVien
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvDSGiaoVien.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvDSGiaoVien.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvDSGiaoVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvDSGiaoVien.DefaultCellStyle = dataGridViewCellStyle3;
            dgvDSGiaoVien.GridColor = Color.FromArgb(231, 229, 255);
            dgvDSGiaoVien.Location = new Point(38, 85);
            dgvDSGiaoVien.Name = "dgvDSGiaoVien";
            dgvDSGiaoVien.RowHeadersVisible = false;
            dgvDSGiaoVien.RowHeadersWidth = 51;
            dgvDSGiaoVien.Size = new Size(626, 255);
            dgvDSGiaoVien.TabIndex = 1;
            dgvDSGiaoVien.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvDSGiaoVien.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvDSGiaoVien.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvDSGiaoVien.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvDSGiaoVien.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvDSGiaoVien.ThemeStyle.BackColor = Color.White;
            dgvDSGiaoVien.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvDSGiaoVien.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvDSGiaoVien.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvDSGiaoVien.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvDSGiaoVien.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvDSGiaoVien.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDSGiaoVien.ThemeStyle.HeaderStyle.Height = 4;
            dgvDSGiaoVien.ThemeStyle.ReadOnly = false;
            dgvDSGiaoVien.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvDSGiaoVien.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDSGiaoVien.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvDSGiaoVien.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvDSGiaoVien.ThemeStyle.RowsStyle.Height = 29;
            dgvDSGiaoVien.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvDSGiaoVien.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // btnLuu
            // 
            btnLuu.BorderRadius = 20;
            btnLuu.CustomizableEdges = customizableEdges1;
            btnLuu.DisabledState.BorderColor = Color.DarkGray;
            btnLuu.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLuu.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLuu.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLuu.Font = new Font("Segoe UI", 9F);
            btnLuu.ForeColor = Color.White;
            btnLuu.Location = new Point(502, 399);
            btnLuu.Name = "btnLuu";
            btnLuu.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLuu.Size = new Size(162, 47);
            btnLuu.TabIndex = 23;
            btnLuu.Text = "Lưu thay đổi";
            btnLuu.Click += btnLuu_Click;
            // 
            // btnHuy
            // 
            btnHuy.BorderRadius = 20;
            btnHuy.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            btnHuy.CustomizableEdges = customizableEdges3;
            btnHuy.DisabledState.BorderColor = Color.DarkGray;
            btnHuy.DisabledState.CustomBorderColor = Color.DarkGray;
            btnHuy.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnHuy.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnHuy.FillColor = Color.Silver;
            btnHuy.Font = new Font("Segoe UI", 9F);
            btnHuy.ForeColor = Color.White;
            btnHuy.Location = new Point(370, 399);
            btnHuy.Name = "btnHuy";
            btnHuy.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnHuy.Size = new Size(105, 47);
            btnHuy.TabIndex = 22;
            btnHuy.Text = "Hủy";
            btnHuy.Click += btnHuy_Click;
            // 
            // FormAddTeacherClass
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(708, 516);
            Controls.Add(btnLuu);
            Controls.Add(btnHuy);
            Controls.Add(dgvDSGiaoVien);
            Controls.Add(label1);
            Name = "FormAddTeacherClass";
            Text = "FormAddTeacherClass";
            ((System.ComponentModel.ISupportInitialize)dgvDSGiaoVien).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Guna.UI2.WinForms.Guna2DataGridView dgvDSGiaoVien;
        private Guna.UI2.WinForms.Guna2Button btnLuu;
        private Guna.UI2.WinForms.Guna2Button btnHuy;
    }
}