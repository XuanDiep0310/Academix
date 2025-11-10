namespace Academix.WinApp.Forms.Teacher
{
    partial class UC_MyClasses
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
            flowPanelClasses = new FlowLayoutPanel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            SuspendLayout();
            // 
            // flowPanelClasses
            // 
            flowPanelClasses.AutoScroll = true;
            flowPanelClasses.Dock = DockStyle.Bottom;
            flowPanelClasses.Location = new Point(0, 107);
            flowPanelClasses.Name = "flowPanelClasses";
            flowPanelClasses.Size = new Size(1165, 597);
            flowPanelClasses.TabIndex = 0;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            guna2HtmlLabel1.Location = new Point(36, 27);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(143, 30);
            guna2HtmlLabel1.TabIndex = 1;
            guna2HtmlLabel1.Text = "Lớp học của tôi";
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 163);
            guna2HtmlLabel2.ForeColor = SystemColors.ControlDarkDark;
            guna2HtmlLabel2.Location = new Point(36, 63);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(300, 25);
            guna2HtmlLabel2.TabIndex = 2;
            guna2HtmlLabel2.Text = "Danh sách các lớp bạn đang giảng dạy";
            // 
            // UC_MyClasses
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            Controls.Add(guna2HtmlLabel2);
            Controls.Add(guna2HtmlLabel1);
            Controls.Add(flowPanelClasses);
            Name = "UC_MyClasses";
            Size = new Size(1165, 704);
            Load += UC_MyClasses_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowPanelClasses;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
    }
}
