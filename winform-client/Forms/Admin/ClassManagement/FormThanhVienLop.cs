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
        private readonly int _classId;
        private readonly string _className; 
        private readonly ClassApiService _classApi;

        public FormThanhVienLop(int classId, string className)
        {
            InitializeComponent();

            _classId = classId;
            _className = className; 
            _classApi = new ClassApiService();

            lblClassCode.Text = $"Danh sách lớp: {_className}";
            SetupDataGridViews();

            this.Load += FormThanhVienLop_Load;
        }
        private void SetupDataGridViews()
        {
            // Giáo viên
            dgvGiaoVien.Columns.Clear();
            dgvGiaoVien.AutoGenerateColumns = false;
            dgvGiaoVien.Columns.Add("FullName", "Họ và tên");
            dgvGiaoVien.Columns.Add("Role", "Vai trò");

            // Học sinh
            dgvHocSinh.Columns.Clear();
            dgvHocSinh.AutoGenerateColumns = false;
            dgvHocSinh.Columns.Add("FullName", "Họ và tên");
            dgvHocSinh.Columns.Add("Role", "Vai trò");
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
                var teachers = await _classApi.GetTeachersAsync(_classId);
                dgvGiaoVien.Rows.Clear();

                foreach (var t in teachers)
                {
                    dgvGiaoVien.Rows.Add(t.FullName, t.Role);
                }

                // Lấy danh sách học sinh
                var students = await _classApi.GetStudentsAsync(_classId);
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

        private async void btnThemGiaoVien_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormAddTeacherClass(_classId); // truyền classId nếu cần
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMembersAsync(); // reload danh sách giáo viên sau khi thêm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm giáo viên: " + ex.Message);
            }
        }

        private async void btnThemHocSinh_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormAddStudentClass(_classId); // truyền classId nếu cần
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMembersAsync(); // reload danh sách học sinh sau khi thêm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm học sinh: " + ex.Message);
            }
        }

    }
}
