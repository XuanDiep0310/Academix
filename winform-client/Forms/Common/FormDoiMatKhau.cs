using Academix.WinApp.Api;
using Academix.WinApp.Models.Auth;
using Academix.WinApp.Utils;
using System;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormDoiMatKhau : Form
    {
        private readonly AuthApi _authApi;

        public FormDoiMatKhau()
        {
            InitializeComponent();
            _authApi = new AuthApi(Config.GetApiBaseUrl());
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form
        }

        private async void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirmPass = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Không được để trống các trường.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không trùng khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnDoiMatKhau.Enabled = false;

            try
            {
                var request = new ChangePasswordRequest
                {
                    OldPassword = oldPass,
                    NewPassword = newPass,
                    ConfirmPassword = confirmPass
                };

                var result = await _authApi.ChangePasswordAsync(request);

                if (result.Success)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message ?? "Đổi mật khẩu thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gọi API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDoiMatKhau.Enabled = true;
            }
        }
    }
}
