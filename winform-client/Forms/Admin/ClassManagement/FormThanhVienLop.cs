using Academix.WinApp.Api;
using Academix.WinApp.Models.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    public partial class FormThanhVienLop : Form
    {
        private readonly string _classCode;
        private readonly ClassApiService _classApi;

        public FormThanhVienLop(string classCode)
        {
            InitializeComponent();
            _classCode = classCode;
            _classApi = new ClassApiService();

            // lblClassCode.Text = $"Danh sách lớp: {_classCode}";
            // Load dữ liệu khi form load
            this.Load += FormThanhVienLop_Load;
        }

        private async void FormThanhVienLop_Load(object sender, EventArgs e)
        {
            await LoadMembersAsync();
        }

        private async Task LoadMembersAsync()
        {
            try
            {
                // Lấy danh sách giáo viên
                var teachers = await _classApi.GetTeachersAsync(_classCode);
                dgvGiaoVien.Rows.Clear();
                foreach (var t in teachers)
                {
                    dgvGiaoVien.Rows.Add(t.FullName, t.Role);
                }

                // Lấy danh sách học sinh
                var students = await _classApi.GetStudentsAsync(_classCode);
                dgvHocSinh.Rows.Clear();
                foreach (var s in students)
                {
                    dgvHocSinh.Rows.Add(s.FullName, s.Role);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load thành viên: " + ex.Message);
            }
        }

        private void btnThemGiaoVien_Click(object sender, EventArgs e)
        {
            // Thêm giáo viên: mở form thêm, sau đó reload dữ liệu
        }

        private void btnThemHocSinh_Click(object sender, EventArgs e)
        {
            // Thêm học sinh: mở form thêm, sau đó reload dữ liệu
        }
    }
}
