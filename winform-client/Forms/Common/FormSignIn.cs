using Academix.WinApp.Api;
using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms.Teacher;
using Academix.WinApp.Models;
using Academix.WinApp.Models.Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Windows.Forms;
using Academix.WinApp.Forms.Student;

namespace Academix.WinApp.Forms
{
    public partial class FormSignIn : Form
    {
        private readonly AuthApi _authApi;

        public FormSignIn()
        {
            InitializeComponent();

            _authApi = new AuthApi(Config.GetApiBaseUrl());

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

                 var loginResult = await _authApi.LoginAsync(loginRequest);

                if (loginResult.Success)
                {
                    SessionManager.Token = loginResult.Token;
                    SessionManager.RefreshToken = loginResult.RefreshToken;
                    SessionManager.CurrentUser = loginResult.User;

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    OpenMainForm(loginResult.User?.Role); // dùng ?. để tránh null
                    this.Hide();
                }



                else
                {
                    MessageBox.Show(loginResult.Message, "Đăng nhập thất bại",
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
                case "Student":
                    mainForm = new FormMainStudent();
                    break;
                default:
                    MessageBox.Show("Role không hợp lệ!", "Lỗi");
                    return;
            }

            mainForm.FormClosed += (s, e) => this.Close();
            mainForm.Show();
        }
    }
}