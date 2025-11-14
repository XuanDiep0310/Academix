using Academix.WinApp.Api;
using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormThemGiaoVien : Form
    {
        private readonly UserApi _userApi;

        public FormThemGiaoVien()
        {
            InitializeComponent();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));
        }

        private async void btnTao_Click(object sender, EventArgs e)
        {
            try
            {
                // Giả sử bạn có các textbox tên: txtEmail, txtPassword, txtFullName
                // và dropdown cboRole để chọn role (ví dụ: Teacher)
                var request = new UserCreateRequest
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtMatKhau.Text.Trim(),
                    FullName = txtHoTen.Text.Trim(),
                    Role = "Teacher"
                };

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Email và Mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnTao.Enabled = false;
                btnTao.Text = "Đang tạo...";

                var result = await _userApi.CreateUserAsync(request);

                if (result.Success)
                {
                    MessageBox.Show($"Tạo tài khoản thành công cho giáo viên: {request.FullName}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Không thể tạo tài khoản.\n{result.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTao.Enabled = true;
                btnTao.Text = "Tạo";
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
