using Academix.WinApp.Api;
using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormThemHocSinh : Form
    {
        private readonly UserBulkApi _userBulkApi;

        public FormThemHocSinh()
        {
            InitializeComponent();
            _userBulkApi = new UserBulkApi(Config.Get("ApiSettings:BaseUrl"));
        }

        private async void btnTao_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtDSHocSinh.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    MessageBox.Show("Vui lòng nhập danh sách học sinh.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse từng dòng theo định dạng: email,họ tên,mật khẩu
                var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                var users = new List<UserBulkModel>();

                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 3)
                    {
                        MessageBox.Show($"Dòng không hợp lệ: \"{line}\".\nĐịnh dạng phải là: email,họ tên,mật khẩu", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    users.Add(new UserBulkModel
                    {
                        email = parts[0].Trim(),
                        fullName = parts[1].Trim(),
                        password = parts[2].Trim(),
                        role = "Student"
                    });
                }

                // Gọi API
                string response = await _userBulkApi.AddUsersBulkAsync(users);

                MessageBox.Show("Thêm học sinh thành công!\n\nKết quả:\n" + response, "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;   
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm học sinh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
