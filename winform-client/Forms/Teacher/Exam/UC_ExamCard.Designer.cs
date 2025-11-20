namespace Academix.WinApp.Forms.Teacher
{
    partial class UC_ExamCard
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnSua = new Guna.UI2.WinForms.Guna2Button();
            lblLop = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblKiemTra = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTrangThai = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblThoiGian = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblSoLuong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblKetThuc = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblBatDau = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnXoa = new Guna.UI2.WinForms.Guna2Button();
            btnCongBo = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // btnSua
            // 
            btnSua.BorderRadius = 15;
            btnSua.BorderThickness = 1;
            btnSua.CustomizableEdges = customizableEdges1;
            btnSua.DisabledState.BorderColor = Color.DarkGray;
            btnSua.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSua.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSua.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSua.FillColor = Color.Transparent;
            btnSua.Font = new Font("Segoe UI", 9F);
            btnSua.ForeColor = Color.Black;
            btnSua.Location = new Point(874, 28);
            btnSua.Name = "btnSua";
            btnSua.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnSua.Size = new Size(56, 35);
            btnSua.TabIndex = 12;
            btnSua.Text = "Sửa";
            btnSua.Click += btnSua_Click;
            // 
            // lblLop
            // 
            lblLop.BackColor = Color.Transparent;
            lblLop.BorderStyle = BorderStyle.FixedSingle;
            lblLop.Location = new Point(45, 67);
            lblLop.Name = "lblLop";
            lblLop.Size = new Size(30, 24);
            lblLop.TabIndex = 10;
            lblLop.Text = "Lớp";
            // 
            // lblKiemTra
            // 
            lblKiemTra.BackColor = Color.Transparent;
            lblKiemTra.Font = new Font("Segoe UI Semibold", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblKiemTra.Location = new Point(44, 28);
            lblKiemTra.Name = "lblKiemTra";
            lblKiemTra.Size = new Size(389, 33);
            lblKiemTra.TabIndex = 9;
            lblKiemTra.Text = "Kiểm tra Đại số tuyến tính - Chương 1";
            // 
            // lblTrangThai
            // 
            lblTrangThai.BackColor = Color.Transparent;
            lblTrangThai.BorderStyle = BorderStyle.FixedSingle;
            lblTrangThai.Location = new Point(331, 67);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(72, 24);
            lblTrangThai.TabIndex = 14;
            lblTrangThai.Text = "Trạng thái";
            // 
            // lblThoiGian
            // 
            lblThoiGian.BackColor = Color.Transparent;
            lblThoiGian.Font = new Font("Segoe UI", 10.8F);
            lblThoiGian.ForeColor = SystemColors.ButtonShadow;
            lblThoiGian.Location = new Point(45, 125);
            lblThoiGian.Name = "lblThoiGian";
            lblThoiGian.Size = new Size(76, 27);
            lblThoiGian.TabIndex = 15;
            lblThoiGian.Text = "Thời gian";
            // 
            // lblSoLuong
            // 
            lblSoLuong.BackColor = Color.Transparent;
            lblSoLuong.Font = new Font("Segoe UI", 10.8F);
            lblSoLuong.ForeColor = SystemColors.ButtonShadow;
            lblSoLuong.Location = new Point(496, 125);
            lblSoLuong.Name = "lblSoLuong";
            lblSoLuong.Size = new Size(86, 27);
            lblSoLuong.TabIndex = 16;
            lblSoLuong.Text = "Số câu hỏi";
            // 
            // lblKetThuc
            // 
            lblKetThuc.BackColor = Color.Transparent;
            lblKetThuc.Font = new Font("Segoe UI", 10.8F);
            lblKetThuc.ForeColor = SystemColors.ButtonShadow;
            lblKetThuc.Location = new Point(44, 218);
            lblKetThuc.Name = "lblKetThuc";
            lblKetThuc.Size = new Size(67, 27);
            lblKetThuc.TabIndex = 17;
            lblKetThuc.Text = "Kết thúc";
            // 
            // lblBatDau
            // 
            lblBatDau.BackColor = Color.Transparent;
            lblBatDau.Font = new Font("Segoe UI", 10.8F);
            lblBatDau.ForeColor = SystemColors.ButtonShadow;
            lblBatDau.Location = new Point(44, 181);
            lblBatDau.Name = "lblBatDau";
            lblBatDau.Size = new Size(67, 27);
            lblBatDau.TabIndex = 18;
            lblBatDau.Text = "Bắt đầu:";
            // 
            // btnXoa
            // 
            btnXoa.BorderRadius = 15;
            btnXoa.BorderThickness = 1;
            btnXoa.CustomizableEdges = customizableEdges3;
            btnXoa.DisabledState.BorderColor = Color.DarkGray;
            btnXoa.DisabledState.CustomBorderColor = Color.DarkGray;
            btnXoa.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnXoa.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnXoa.FillColor = Color.Transparent;
            btnXoa.Font = new Font("Segoe UI", 9F);
            btnXoa.ForeColor = Color.Black;
            btnXoa.Location = new Point(1036, 28);
            btnXoa.Name = "btnXoa";
            btnXoa.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnXoa.Size = new Size(91, 35);
            btnXoa.TabIndex = 19;
            btnXoa.Text = "Xóa";
            btnXoa.Click += guna2Button1_Click;
            // 
            // btnCongBo
            // 
            btnCongBo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCongBo.BorderRadius = 15;
            btnCongBo.CustomizableEdges = customizableEdges5;
            btnCongBo.DisabledState.BorderColor = Color.DarkGray;
            btnCongBo.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCongBo.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCongBo.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCongBo.FillColor = Color.DodgerBlue;
            btnCongBo.Font = new Font("Segoe UI", 9F);
            btnCongBo.ForeColor = Color.White;
            btnCongBo.Location = new Point(936, 28);
            btnCongBo.Name = "btnCongBo";
            btnCongBo.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnCongBo.Size = new Size(94, 36);
            btnCongBo.TabIndex = 20;
            btnCongBo.Text = "Công bố";
            btnCongBo.Click += btnCongBo_Click;
            // 
            // UC_ExamCard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(btnCongBo);
            Controls.Add(btnXoa);
            Controls.Add(lblBatDau);
            Controls.Add(lblKetThuc);
            Controls.Add(lblSoLuong);
            Controls.Add(lblThoiGian);
            Controls.Add(lblTrangThai);
            Controls.Add(btnSua);
            Controls.Add(lblLop);
            Controls.Add(lblKiemTra);
            Name = "UC_ExamCard";
            Size = new Size(1161, 263);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Guna.UI2.WinForms.Guna2Button btnSua;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLop;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblKiemTra;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTrangThai;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblThoiGian;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoLuong;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblKetThuc;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBatDau;
        private Guna.UI2.WinForms.Guna2Button btnXoa;
        private Guna.UI2.WinForms.Guna2Button btnCongBo;
    }
}
