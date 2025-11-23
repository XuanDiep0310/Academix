namespace Academix.WinApp.Forms.Student
{
    partial class UC_LopHocCuaToi
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
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            layoutMyCard = new FlowLayoutPanel();
            guna2Panel1.SuspendLayout();
            guna2Panel2.SuspendLayout();
            SuspendLayout();
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.ForeColor = SystemColors.ActiveCaptionText;
            guna2HtmlLabel2.Location = new Point(48, 69);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(211, 22);
            guna2HtmlLabel2.TabIndex = 1;
            guna2HtmlLabel2.Text = "Các lớp học bạn đang tham gia";
            guna2HtmlLabel2.Click += guna2HtmlLabel2_Click;
            // 
            // guna2Panel1
            // 
            guna2Panel1.Controls.Add(guna2HtmlLabel1);
            guna2Panel1.Controls.Add(guna2HtmlLabel2);
            guna2Panel1.CustomizableEdges = customizableEdges1;
            guna2Panel1.Dock = DockStyle.Top;
            guna2Panel1.Location = new Point(0, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Panel1.Size = new Size(1162, 116);
            guna2Panel1.TabIndex = 1;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            guna2HtmlLabel1.Location = new Point(48, 24);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(198, 39);
            guna2HtmlLabel1.TabIndex = 2;
            guna2HtmlLabel1.Text = "Lớp học của tôi";
            // 
            // guna2Panel2
            // 
            guna2Panel2.AutoScroll = true;
            guna2Panel2.BackColor = Color.FromArgb(249, 250, 251);
            guna2Panel2.Controls.Add(layoutMyCard);
            guna2Panel2.CustomizableEdges = customizableEdges3;
            guna2Panel2.Dock = DockStyle.Fill;
            guna2Panel2.FillColor = Color.FromArgb(249, 250, 251);
            guna2Panel2.Location = new Point(0, 116);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2Panel2.Size = new Size(1162, 654);
            guna2Panel2.TabIndex = 2;
            // 
            // layoutMyCard
            // 
            layoutMyCard.AutoScroll = true;
            layoutMyCard.BackColor = Color.WhiteSmoke;
            layoutMyCard.Dock = DockStyle.Fill;
            layoutMyCard.Location = new Point(0, 0);
            layoutMyCard.Name = "layoutMyCard";
            layoutMyCard.Padding = new Padding(30, 20, 20, 20);
            layoutMyCard.Size = new Size(1162, 654);
            layoutMyCard.TabIndex = 0;
            // 
            // UC_LopHocCuaToi
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(guna2Panel2);
            Controls.Add(guna2Panel1);
            Name = "UC_LopHocCuaToi";
            Size = new Size(1162, 770);
            Load += UC_LopHocCuaToi_Load;
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            guna2Panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private FlowLayoutPanel layoutMyCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
    }
}
