using Academix.WinApp.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormTaoLopHoc : Form
    {
        private readonly ClassApiService _classApiService;
        public FormTaoLopHoc()
        {
            InitializeComponent();
            _classApiService = new ClassApiService();
        }

        private async void btnTao_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ các TextBox trên form
                string className = txtTenLop.Text.Trim();
                string classCode = txtMaLop.Text.Trim();
                string description = txtMoTa.Text.Trim();

                // Kiểm tra dữ liệu hợp lệ
                if (string.IsNullOrEmpty(className))
                {
                    MessageBox.Show("Vui lòng nhập tên lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(classCode))
                {
                    MessageBox.Show("Vui lòng nhập mã lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi API tạo lớp học
                var newClass = await _classApiService.CreateClassAsync(className, classCode, description);

                MessageBox.Show($"Tạo lớp thành công: {newClass.ClassName}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Đặt DialogResult OK để form gọi biết tạo thành công
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

       
    }
}
