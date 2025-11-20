namespace Academix.WinApp.Forms.Student.Exam
{
    partial class UC_DoExamCard
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
            lblCauHoi = new Guna.UI2.WinForms.Guna2HtmlLabel();
            flowOptions = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // lblCauHoi
            // 
            lblCauHoi.BackColor = Color.Transparent;
            lblCauHoi.Font = new Font("Segoe UI Semibold", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lblCauHoi.Location = new Point(24, 18);
            lblCauHoi.Name = "lblCauHoi";
            lblCauHoi.Size = new Size(130, 33);
            lblCauHoi.TabIndex = 9;
            lblCauHoi.Text = "Câu hỏi: abc";
            // 
            // flowOptions
            // 
            flowOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            flowOptions.AutoSize = true;
            flowOptions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowOptions.FlowDirection = FlowDirection.TopDown;
            flowOptions.Location = new Point(24, 68);
            flowOptions.Margin = new Padding(3, 10, 3, 3);
            flowOptions.Name = "flowOptions";
            flowOptions.Size = new Size(0, 0);
            flowOptions.TabIndex = 16;
            flowOptions.WrapContents = false;
            // 
            // UC_DoExamCard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(flowOptions);
            Controls.Add(lblCauHoi);
            Name = "UC_DoExamCard";
            Padding = new Padding(20);
            Size = new Size(890, 120);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblCauHoi;
        private FlowLayoutPanel flowOptions;
    }
}
