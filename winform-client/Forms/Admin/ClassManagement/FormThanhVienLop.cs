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

            // Cột thao tác (xóa)
            DataGridViewButtonColumn btnDeleteTeacher = new DataGridViewButtonColumn();
            btnDeleteTeacher.HeaderText = "Thao tác";
            btnDeleteTeacher.Text = "Xóa";
            btnDeleteTeacher.UseColumnTextForButtonValue = true;
            btnDeleteTeacher.Width = 80;
            dgvGiaoVien.Columns.Add(btnDeleteTeacher);


            // Học sinh
            dgvHocSinh.Columns.Clear();
            dgvHocSinh.AutoGenerateColumns = false;

            dgvHocSinh.Columns.Add("FullName", "Họ và tên");
            dgvHocSinh.Columns.Add("Role", "Vai trò");

            // Cột thao tác (xóa)
            DataGridViewButtonColumn btnDeleteStudent = new DataGridViewButtonColumn();
            btnDeleteStudent.HeaderText = "Thao tác";
            btnDeleteStudent.Text = "Xóa";
            btnDeleteStudent.UseColumnTextForButtonValue = true;
            btnDeleteStudent.Width = 80;
            dgvHocSinh.Columns.Add(btnDeleteStudent);

            dgvHocSinh.AllowUserToAddRows = false;
            dgvGiaoVien.AllowUserToAddRows = false;

        }


        private async void FormThanhVienLop_Load(object sender, EventArgs e)
        {
            await LoadMembersAsync();
            dgvGiaoVien.CellClick += dgvGiaoVien_CellClick;
            dgvHocSinh.CellClick += dgvHocSinh_CellClick;

        }
        private async void dgvGiaoVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // click ngoài vùng hợp lệ
            if (e.RowIndex < 0) return;

            // cột Thao tác là cột cuối (index = 2)
            if (e.ColumnIndex == 2)
            {
                string fullName = dgvGiaoVien.Rows[e.RowIndex].Cells[0].Value.ToString();
                int userId = ((ClassMember)dgvGiaoVien.Rows[e.RowIndex].Tag).UserId;

                if (MessageBox.Show($"Xóa giáo viên '{fullName}' khỏi lớp?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                try
                {
                    await _classApi.RemoveMemberAsync(_classId, userId);
                    MessageBox.Show("Đã xóa thành công!");
                    await LoadMembersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
        }
        private async void dgvHocSinh_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 2)
            {
                string fullName = dgvHocSinh.Rows[e.RowIndex].Cells[0].Value.ToString();
                int userId = ((ClassMember)dgvHocSinh.Rows[e.RowIndex].Tag).UserId;

                if (MessageBox.Show($"Xóa học sinh '{fullName}' khỏi lớp?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                try
                {
                    await _classApi.RemoveMemberAsync(_classId, userId);
                    MessageBox.Show("Đã xóa thành công!");
                    await LoadMembersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
            }
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
                    int index = dgvGiaoVien.Rows.Add(t.FullName, t.Role);
                    dgvGiaoVien.Rows[index].Tag = t;
                }


                // Lấy danh sách học sinh
                var students = await _classApi.GetStudentsAsync(_classId);
                dgvHocSinh.Rows.Clear();
                foreach (var s in students)
                {
                    int index = dgvHocSinh.Rows.Add(s.FullName, s.Role);
                    dgvHocSinh.Rows[index].Tag = s;   // ⭐ GÁN TAG ĐỂ LẤY USERID
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
