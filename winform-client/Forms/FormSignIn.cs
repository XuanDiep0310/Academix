using Academix.WinApp.Api;
using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms.Teacher;
using Academix.WinApp.Models;
using Academix.WinApp.Models.Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Windows.Forms;

namespace Academix.WinApp.Forms
{
    public partial class FormSignIn : Form
    {
        private readonly AuthApi _authApi;

        public FormSignIn()
        {
            InitializeComponent();
            _authApi = new AuthApi();
        }

        private async void btnDangNhap_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            // Disable button
            btnDangNhap.Enabled = false;
            btnDangNhap.Text = "Đang đăng nhập...";
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtMatKhau.Text
                };

                var result = await _authApi.LoginAsync(loginRequest);

                if (result.Success)
                {
                    // Lưu session
                    SessionManager.Token = result.Token;
                    SessionManager.RefreshToken = result.RefreshToken;
                    SessionManager.CurrentUser = result.User;

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở form tương ứng
                    OpenMainForm(result.User.Role);
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(result.Message, "Đăng nhập thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Có lỗi xảy ra:\n{ex.Message}\n\nVui lòng kiểm tra:\n" +
                    "1. API có đang chạy không?\n" +
                    "2. Đường dẫn API có đúng không?",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnDangNhap.Enabled = true;
                btnDangNhap.Text = "Đăng nhập";
                this.Cursor = Cursors.Default;
            }
        }

        private void OpenMainForm(string Role)
        {
            Form mainForm = null;

            switch (Role)
            {
                case "Admin":
                    mainForm = new FormMainAdmin();
                    break;
                case "Teacher":
                    mainForm = new FormMainTeacher();
                    break;
                //case "student":
                //mainForm = new StudentForm();
                //break;
                default:
                    MessageBox.Show("Role không hợp lệ!", "Lỗi");
                    return;
            }

            mainForm.FormClosed += (s, e) => this.Close();
            mainForm.Show();
        }
    }
}