using Academix.WinApp.Api;
using Academix.WinApp.Models; // để dùng UserData
using System;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormEditUser : Form
    {
        private readonly UserData _user;

        // Constructor mặc định (designer vẫn cần)
        public FormEditUser()
        {
            InitializeComponent();
        }

        // Constructor mới nhận UserData
        public FormEditUser(UserData user) : this() // gọi constructor mặc định trước
        {
            _user = user;

            // Gán dữ liệu lên TextBox hoặc controls
            txtHoTen.Text = _user.FullName;
            txtEmail.Text = _user.Email;
            // thêm các control khác nếu cần
        }

        private async void btnCapNhat_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ các control
                string fullName = txtHoTen.Text.Trim();
                string email = txtEmail.Text.Trim();
                

                // Validate cơ bản
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ họ tên và email.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo instance API
                var userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));

                // Gọi API update user
                var response = await userApi.UpdateUserAsync(_user.UserId, fullName, email);

                if (response.Success)
                {
                    MessageBox.Show($"Cập nhật thành công: {response.Data.FullName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // báo form cha biết update thành công
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Cập nhật thất bại: {response.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật user: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
