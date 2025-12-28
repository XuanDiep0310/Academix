using Academix.WinApp.Api;
using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms.Teacher;
using Academix.WinApp.Models;
using Academix.WinApp.Models.Academix.WinApp.Models;
using Academix.WinApp.Utils;
using Academix.WinApp.Languages;
using System;
using System.Windows.Forms;
using Academix.WinApp.Forms.Student;

namespace Academix.WinApp.Forms
{
    public partial class FormSignIn : Form
    {
        private readonly AuthApi _authApi;
        private Form _currentMainForm; // Lưu form chính hiện tại

        public FormSignIn()
        {
            InitializeComponent();

            _authApi = new AuthApi(Config.GetApiBaseUrl());
            
            // Load ngôn ngữ
            LoadLanguage();
        }

        private void LoadLanguage()
        {
            guna2GroupBox1.Text = LanguageManager.GetString("LoginTitle");
            guna2HtmlLabel3.Text = LanguageManager.GetString("WelcomeBack");
            guna2HtmlLabel6.Text = LanguageManager.GetString("Email");
            guna2HtmlLabel5.Text = LanguageManager.GetString("Password");
            btnDangNhap.Text = LanguageManager.GetString("Login");
            btnForgotPassWord.Text = LanguageManager.GetString("ForgotPassword");
            
            // Cập nhật nút ngôn ngữ
            if (btnLanguage != null)
            {
                btnLanguage.Text = LanguageManager.CurrentLanguage.ToUpper();
            }
            
            // Placeholder cho textbox
            txtEmail.PlaceholderText = LanguageManager.GetString("Email");
            txtMatKhau.PlaceholderText = LanguageManager.GetString("Password");
        }


        private async void btnDangNhap_Click_1(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show(
                    LanguageManager.GetString("PleaseEnterEmail"), 
                    LanguageManager.GetString("Notification"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show(
                    LanguageManager.GetString("PleaseEnterPassword"), 
                    LanguageManager.GetString("Notification"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            // Disable button
            btnDangNhap.Enabled = false;
            btnDangNhap.Text = LanguageManager.GetString("LoggingIn");
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtMatKhau.Text
                };

                var loginResult = await _authApi.LoginAsync(loginRequest);

                if (loginResult.Success)
                {
                    // Đảm bảo session được set trước khi mở form chính
                    SessionManager.Token = loginResult.Token;
                    SessionManager.RefreshToken = loginResult.RefreshToken;
                    SessionManager.CurrentUser = loginResult.User;

                    // Kiểm tra lại session trước khi mở form
                    if (SessionManager.CurrentUser == null || string.IsNullOrEmpty(SessionManager.Token))
                    {
                        MessageBox.Show(
                            LanguageManager.GetString("InvalidSession"), 
                            LanguageManager.GetString("Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    

                    // Hiển thị thông báo thành công (không chặn)
                    MessageBox.Show(
                        LanguageManager.GetString("LoginSuccess"), 
                        LanguageManager.GetString("Notification"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở form chính
                    OpenMainForm(loginResult.User?.Role);
                }
                else
                {
                    MessageBox.Show(
                        loginResult.Message, 
                        LanguageManager.GetString("LoginFailed"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(LanguageManager.GetString("NetworkError"), ex.Message);
                MessageBox.Show(
                    errorMsg,
                    LanguageManager.GetString("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnDangNhap.Enabled = true;
                btnDangNhap.Text = LanguageManager.GetString("Login");
                this.Cursor = Cursors.Default;
            }
        }

        private void OpenMainForm(string Role)
        {
            try
            {
                // Đóng form chính cũ nếu có (tránh nhiều form cùng mở)
                if (_currentMainForm != null && !_currentMainForm.IsDisposed)
                {
                    _currentMainForm.FormClosed -= MainForm_FormClosed; // Gỡ event handler cũ
                    _currentMainForm.Close();
                    _currentMainForm.Dispose();
                }

                Form mainForm = null;

                switch (Role)
                {
                    case "Admin":
                        mainForm = new FormMainAdmin();
                        break;
                    case "Teacher":
                        mainForm = new FormMainTeacher();
                        break;
                    case "Student":
                        mainForm = new FormMainStudent();
                        break;
                    default:
                        MessageBox.Show(
                            LanguageManager.GetString("InvalidRole"), 
                            LanguageManager.GetString("Error"), 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                if (mainForm != null)
                {
                    // Lưu reference form chính
                    _currentMainForm = mainForm;

                    // Khi form chính đóng, hiển thị lại form đăng nhập
                    mainForm.FormClosed += MainForm_FormClosed;
                    
                    // ẨN form đăng nhập (không đóng) để có thể hiển thị lại nhanh khi đăng xuất
                    // Giữ lại state (ngôn ngữ, etc.) và không cần tạo lại form
                    this.Hide();
                    
                    // Show form chính và đảm bảo nó được focus
                    mainForm.Show();
                    mainForm.WindowState = FormWindowState.Normal;
                    mainForm.BringToFront();
                    mainForm.Activate();
                    mainForm.Focus();
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(LanguageManager.GetString("ErrorOpeningForm"), ex.Message);
                MessageBox.Show(
                    errorMsg, 
                    LanguageManager.GetString("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Hiển thị lại form đăng nhập nếu có lỗi
                if (!this.IsDisposed)
                {
                    this.Show();
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Clear reference
            _currentMainForm = null;

            // Hiển thị lại form đăng nhập (form này luôn được ẨN, không đóng)
            // Điều này giúp giữ lại state và hiển thị lại nhanh chóng
            if (!this.IsDisposed)
            {
                // Xóa thông tin đăng nhập
                txtEmail.Text = "";
                txtMatKhau.Text = "";
                
                // Hiển thị lại form
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Activate();
                txtEmail.Focus();
            }
        }

        private void btnForgotPassWord_Click_1(object sender, EventArgs e)
        {
            var forgotForm = new Academix.WinApp.Forms.Common.FormForgotPassword();
            forgotForm.StartPosition = FormStartPosition.CenterParent; // hiện giữa màn hình
            forgotForm.ShowDialog(); // Show dạng popup
        }


        private void btnEye_Click_1(object sender, EventArgs e)
        {
            // Toggle mật khẩu cho txtMatKhau
            txtMatKhau.UseSystemPasswordChar = !txtMatKhau.UseSystemPasswordChar;

            btnEye.Image = txtMatKhau.UseSystemPasswordChar
                            ? Properties.Resources.eye_closed
                            : Properties.Resources.eye_open;
        }

        private void FormSignIn_Load(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = true;
            
            // Thêm Enter key để đăng nhập
            txtEmail.KeyDown += (s, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    txtMatKhau.Focus();
                }
            };
            
            txtMatKhau.KeyDown += (s, args) =>
            {
                if (args.KeyCode == Keys.Enter && btnDangNhap.Enabled)
                {
                    btnDangNhap_Click_1(sender, e);
                }
            };
            
            // Focus vào txtEmail khi form load
            txtEmail.Focus();
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void btnLanguage_Click(object sender, EventArgs e)
        {
            // Toggle giữa tiếng Việt và tiếng Anh
            if (LanguageManager.CurrentLanguage == "vi")
            {
                LanguageManager.ChangeLanguage("en");
            }
            else
            {
                LanguageManager.ChangeLanguage("vi");
            }
            
            // Reload ngôn ngữ
            LoadLanguage();
        }

        /// <summary>
        /// Hiển thị lại form đăng nhập và reset thông tin
        /// </summary>
        public void ShowLoginForm()
        {
            // Xóa thông tin đăng nhập
            txtEmail.Text = "";
            txtMatKhau.Text = "";
            
            // Hiển thị form
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();
            txtEmail.Focus();
        }

    }
}