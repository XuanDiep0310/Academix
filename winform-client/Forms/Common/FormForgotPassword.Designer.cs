namespace Academix.WinApp.Forms.Common
{
    partial class FormForgotPassword
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2GroupBox1 = new Guna.UI2.WinForms.Guna2GroupBox();
            btnLayLaiMatKhau = new Guna.UI2.WinForms.Guna2Button();
            txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2GroupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2GroupBox1
            // 
            guna2GroupBox1.Controls.Add(btnLayLaiMatKhau);
            guna2GroupBox1.Controls.Add(txtEmail);
            guna2GroupBox1.Controls.Add(guna2HtmlLabel1);
            guna2GroupBox1.CustomizableEdges = customizableEdges5;
            guna2GroupBox1.Font = new Font("Segoe UI", 9F);
            guna2GroupBox1.ForeColor = Color.Black;
            guna2GroupBox1.Location = new Point(192, 31);
            guna2GroupBox1.Name = "guna2GroupBox1";
            guna2GroupBox1.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2GroupBox1.Size = new Size(401, 413);
            guna2GroupBox1.TabIndex = 1;
            guna2GroupBox1.Text = "Quên mật khẩu";
            guna2GroupBox1.TextAlign = HorizontalAlignment.Center;
            // 
            // btnLayLaiMatKhau
            // 
            btnLayLaiMatKhau.CustomizableEdges = customizableEdges1;
            btnLayLaiMatKhau.DisabledState.BorderColor = Color.DarkGray;
            btnLayLaiMatKhau.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLayLaiMatKhau.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLayLaiMatKhau.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLayLaiMatKhau.Font = new Font("Segoe UI", 9F);
            btnLayLaiMatKhau.ForeColor = Color.White;
            btnLayLaiMatKhau.Location = new Point(32, 237);
            btnLayLaiMatKhau.Name = "btnLayLaiMatKhau";
            btnLayLaiMatKhau.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLayLaiMatKhau.Size = new Size(343, 42);
            btnLayLaiMatKhau.TabIndex = 4;
            btnLayLaiMatKhau.Text = "Lấy lại mật khẩu";
            btnLayLaiMatKhau.Click += btnLayLaiMatKhau_Click;
            // 
            // txtEmail
            // 
            txtEmail.CustomizableEdges = customizableEdges3;
            txtEmail.DefaultText = "";
            txtEmail.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtEmail.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtEmail.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtEmail.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtEmail.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtEmail.Font = new Font("Segoe UI", 9F);
            txtEmail.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtEmail.Location = new Point(32, 151);
            txtEmail.Margin = new Padding(3, 4, 3, 4);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "";
            txtEmail.SelectedText = "";
            txtEmail.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtEmail.Size = new Size(343, 45);
            txtEmail.TabIndex = 1;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Location = new Point(32, 112);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(40, 22);
            guna2HtmlLabel1.TabIndex = 0;
            guna2HtmlLabel1.Text = "Email";
            // 
            // FormForgotPassword
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(771, 494);
            Controls.Add(guna2GroupBox1);
            Name = "FormForgotPassword";
            Text = "FormForgotPassword";
            guna2GroupBox1.ResumeLayout(false);
            guna2GroupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2GroupBox guna2GroupBox1;
        private Guna.UI2.WinForms.Guna2Button btnLayLaiMatKhau;
        private Guna.UI2.WinForms.Guna2TextBox txtEmail;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
    }
}