using Academix.WinApp.Api;
using Academix.WinApp.Models.Auth;
using Academix.WinApp.Utils;
using System;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Common
{
    public partial class FormForgotPassword : Form
    {
        private readonly AuthApi _authApi;

        public FormForgotPassword()
        {
            InitializeComponent();
            _authApi = new AuthApi(Config.GetApiBaseUrl());
        }

        private async void btnLayLaiMatKhau_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // 🔒 Disable nút + đổi text
            btnLayLaiMatKhau.Enabled = false;
            string oldText = btnLayLaiMatKhau.Text;
            btnLayLaiMatKhau.Text = "Đang gửi...";

            try
            {
                var response = await _authApi.ForgotPasswordAsync(new ForgotPasswordRequest
                {
                    Email = email
                });

                MessageBox.Show(
                    response.Message + "\n\nVui lòng mở email và nhấn vào liên kết Reset Password!",
                    response.Success ? "Thành công" : "Thất bại",
                    MessageBoxButtons.OK,
                    response.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error
                );

                // API OK → đóng form
                if (response.Success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi gửi yêu cầu: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // 🔓 Bật lại nút + restore text
                btnLayLaiMatKhau.Enabled = true;
                btnLayLaiMatKhau.Text = oldText;
            }
        }




        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
