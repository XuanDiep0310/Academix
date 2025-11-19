using Academix.WinApp.Api;
using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    public partial class FormSuaLop : Form
    {
        private readonly int _classId;
        private readonly ClassApiService _classApiService;

        public FormSuaLop(int classId)
        {
            InitializeComponent();
            _classId = classId;
            _classApiService = new ClassApiService();

            this.Load += FormSuaLop_Load;
        }

        private async void FormSuaLop_Load(object sender, EventArgs e)
        {
            // Load dữ liệu lớp từ API
            var classData = await _classApiService.GetClassByIdAsync(_classId);
            txtTenLop.Text = classData.ClassName;
            txtMoTa.Text = classData.Description;
        }

        private async void btnCapNhat_Click(object sender, EventArgs e)
        {
            string className = txtTenLop.Text.Trim();
            string description = txtTenLop.Text.Trim();

            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Vui lòng nhập tên lớp.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await _classApiService.UpdateClassAsync(_classId, className, description, true);

            MessageBox.Show("Cập nhật lớp thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        
    }
}
