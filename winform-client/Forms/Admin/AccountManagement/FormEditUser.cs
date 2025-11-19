using Academix.WinApp.Api;
using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormEditUser : Form
    {
        private readonly UserData _user;
        private readonly UserApi _userApi;

        // Constructor mặc định (designer vẫn cần)
        public FormEditUser()
        {
            InitializeComponent();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));
        }

        // Constructor nhận UserData
        public FormEditUser(UserData user) : this()
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            LoadUserData();
        }

        // Load dữ liệu lên các control
        private void LoadUserData()
        {
            txtHoTen.Text = _user.FullName;
            txtEmail.Text = _user.Email;
            // Nếu có thêm control khác thì gán ở đây
        }

        // Nút Cập nhật
        private async void btnCapNhat_Click(object sender, EventArgs e)
        {
            try
            {
                string fullName = txtHoTen.Text.Trim();
                string email = txtEmail.Text.Trim();

                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ họ tên và email.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi API UpdateUserAsync, gửi kèm Id và IsActive từ _user
                var response = await _userApi.UpdateUserAsync(_user.UserId, fullName, email, _user.IsActive);

                if (response.Success)
                {
                    MessageBox.Show($"Cập nhật thành công: {response.Data.FullName}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Cập nhật thất bại: {response.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật user: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút Hủy
        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
