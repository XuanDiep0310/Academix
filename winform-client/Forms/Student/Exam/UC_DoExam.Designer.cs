namespace Academix.WinApp.Forms.Student.Exam
{
    partial class UC_DoExam
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            lblClock = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTenMon = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTenBaiKtra = new Guna.UI2.WinForms.Guna2HtmlLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            btnNopBai = new Guna.UI2.WinForms.Guna2Button();
            lblSoCauDaTraLoi = new Guna.UI2.WinForms.Guna2HtmlLabel();
            PanelDoExam = new FlowLayoutPanel();
            guna2Panel1.SuspendLayout();
            guna2Panel2.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.Controls.Add(lblClock);
            guna2Panel1.Controls.Add(lblTenMon);
            guna2Panel1.Controls.Add(lblTenBaiKtra);
            guna2Panel1.CustomizableEdges = customizableEdges1;
            guna2Panel1.Dock = DockStyle.Top;
            guna2Panel1.Location = new Point(0, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Panel1.Size = new Size(1125, 115);
            guna2Panel1.TabIndex = 0;
            // 
            // lblClock
            // 
            lblClock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblClock.BackColor = Color.Transparent;
            lblClock.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblClock.Location = new Point(999, 37);
            lblClock.Name = "lblClock";
            lblClock.Size = new Size(91, 33);
            lblClock.TabIndex = 2;
            lblClock.Text = "Đồng hồ";
            // 
            // lblTenMon
            // 
            lblTenMon.BackColor = Color.Transparent;
            lblTenMon.Location = new Point(40, 70);
            lblTenMon.Name = "lblTenMon";
            lblTenMon.Size = new Size(61, 22);
            lblTenMon.TabIndex = 1;
            lblTenMon.Text = "Tên môn";
            // 
            // lblTenBaiKtra
            // 
            lblTenBaiKtra.BackColor = Color.Transparent;
            lblTenBaiKtra.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTenBaiKtra.Location = new Point(40, 19);
            lblTenBaiKtra.Name = "lblTenBaiKtra";
            lblTenBaiKtra.Size = new Size(211, 39);
            lblTenBaiKtra.TabIndex = 0;
            lblTenBaiKtra.Text = "Tên bài kiểm tra";
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // guna2Panel2
            // 
            guna2Panel2.Controls.Add(btnNopBai);
            guna2Panel2.Controls.Add(lblSoCauDaTraLoi);
            guna2Panel2.CustomizableEdges = customizableEdges5;
            guna2Panel2.Dock = DockStyle.Bottom;
            guna2Panel2.Location = new Point(0, 645);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2Panel2.Size = new Size(1125, 119);
            guna2Panel2.TabIndex = 1;
            // 
            // btnNopBai
            // 
            btnNopBai.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNopBai.BorderRadius = 20;
            btnNopBai.CustomizableEdges = customizableEdges3;
            btnNopBai.DisabledState.BorderColor = Color.DarkGray;
            btnNopBai.DisabledState.CustomBorderColor = Color.DarkGray;
            btnNopBai.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnNopBai.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnNopBai.Font = new Font("Segoe UI", 9F);
            btnNopBai.ForeColor = Color.White;
            btnNopBai.Location = new Point(973, 32);
            btnNopBai.Name = "btnNopBai";
            btnNopBai.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnNopBai.Size = new Size(128, 47);
            btnNopBai.TabIndex = 1;
            btnNopBai.Text = "Nộp bài";
            // 
            // lblSoCauDaTraLoi
            // 
            lblSoCauDaTraLoi.BackColor = Color.Transparent;
            lblSoCauDaTraLoi.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSoCauDaTraLoi.Location = new Point(40, 49);
            lblSoCauDaTraLoi.Name = "lblSoCauDaTraLoi";
            lblSoCauDaTraLoi.Size = new Size(157, 30);
            lblSoCauDaTraLoi.TabIndex = 0;
            lblSoCauDaTraLoi.Text = "Đã trả lời: 0/2 câu";
            // 
            // PanelDoExam
            // 
            PanelDoExam.AutoScroll = true;
            PanelDoExam.BackColor = Color.WhiteSmoke;
            PanelDoExam.Dock = DockStyle.Fill;
            PanelDoExam.Location = new Point(0, 115);
            PanelDoExam.Name = "PanelDoExam";
            PanelDoExam.Size = new Size(1125, 530);
            PanelDoExam.TabIndex = 2;
            // 
            // UC_DoExam
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(PanelDoExam);
            Controls.Add(guna2Panel2);
            Controls.Add(guna2Panel1);
            Name = "UC_DoExam";
            Size = new Size(1125, 764);
            Load += UC_DoExam_Load;
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTenMon;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTenBaiKtra;
        private System.Windows.Forms.Timer timer1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblClock;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoCauDaTraLoi;
        private Guna.UI2.WinForms.Guna2Button btnNopBai;
        private FlowLayoutPanel PanelDoExam;
    }
}
