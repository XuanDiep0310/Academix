namespace Academix.WinApp.Forms.Student.MyResult
{
    partial class UC_MyExams
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
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            flowpanelExams = new FlowLayoutPanel();
            guna2Panel2.SuspendLayout();
            guna2Panel3.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.CustomizableEdges = customizableEdges1;
            guna2Panel1.Dock = DockStyle.Bottom;
            guna2Panel1.Location = new Point(0, 622);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Panel1.Size = new Size(1132, 30);
            guna2Panel1.TabIndex = 0;
            // 
            // guna2Panel2
            // 
            guna2Panel2.Controls.Add(guna2HtmlLabel1);
            guna2Panel2.Controls.Add(guna2HtmlLabel2);
            guna2Panel2.CustomizableEdges = customizableEdges3;
            guna2Panel2.Dock = DockStyle.Top;
            guna2Panel2.Location = new Point(0, 0);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2Panel2.Size = new Size(1132, 125);
            guna2Panel2.TabIndex = 1;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            guna2HtmlLabel1.Location = new Point(37, 30);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(132, 33);
            guna2HtmlLabel1.TabIndex = 4;
            guna2HtmlLabel1.Text = "Bài kiểm tra";
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.ForeColor = SystemColors.ActiveCaptionText;
            guna2HtmlLabel2.Location = new Point(37, 75);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(236, 22);
            guna2HtmlLabel2.TabIndex = 3;
            guna2HtmlLabel2.Text = "Danh sách các bài kiểm tra của bạn";
            // 
            // guna2Panel3
            // 
            guna2Panel3.Controls.Add(flowpanelExams);
            guna2Panel3.CustomizableEdges = customizableEdges5;
            guna2Panel3.Dock = DockStyle.Fill;
            guna2Panel3.Location = new Point(0, 125);
            guna2Panel3.Name = "guna2Panel3";
            guna2Panel3.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2Panel3.Size = new Size(1132, 497);
            guna2Panel3.TabIndex = 2;
            // 
            // flowpanelExams
            // 
            flowpanelExams.AutoScroll = true;
            flowpanelExams.AutoSize = true;
            flowpanelExams.Dock = DockStyle.Fill;
            flowpanelExams.Location = new Point(0, 0);
            flowpanelExams.Name = "flowpanelExams";
            flowpanelExams.Size = new Size(1132, 497);
            flowpanelExams.TabIndex = 0;
            flowpanelExams.SizeChanged += flowpanelExams_SizeChanged;
            // 
            // UC_MyExams
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(guna2Panel3);
            Controls.Add(guna2Panel2);
            Controls.Add(guna2Panel1);
            Name = "UC_MyExams";
            Size = new Size(1132, 652);
            Load += UC_MyExams_Load;
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            guna2Panel3.ResumeLayout(false);
            guna2Panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private FlowLayoutPanel flowpanelExams;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
    }
}
