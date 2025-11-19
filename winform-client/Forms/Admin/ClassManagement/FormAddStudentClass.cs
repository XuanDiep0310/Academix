using Academix.WinApp.Api;
using Academix.WinApp.Models.Classes;
using Academix.WinApp.Models.Users;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    public partial class FormAddStudentClass : Form
    {
        private readonly int _classId;
        private readonly ClassApiService _classApi;
        private readonly UserApi _userApi;

        private List<UserData> _allStudents;  // danh sách tất cả học sinh

        public FormAddStudentClass(int classId)
        {
            InitializeComponent();
            _classId = classId;

            _classApi = new ClassApiService();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));

            this.Load += FormAddStudentClass_Load;
        }

        private async void FormAddStudentClass_Load(object sender, EventArgs e)
        {
            InitializeStudentGrid();
            await LoadStudentsAsync();
        }

        #region Khởi tạo DataGridView
        private void InitializeStudentGrid()
        {
            dgvDSHocSinh.AutoGenerateColumns = false;
            dgvDSHocSinh.Columns.Clear();

            // Checkbox column
            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.HeaderText = "Chọn";
            chkCol.Width = 50;
            dgvDSHocSinh.Columns.Add(chkCol);

            // Name column
            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.HeaderText = "Tên học sinh";
            nameCol.DataPropertyName = "FullName";
            nameCol.Width = 200;
            dgvDSHocSinh.Columns.Add(nameCol);

            // Email column
            DataGridViewTextBoxColumn emailCol = new DataGridViewTextBoxColumn();
            emailCol.HeaderText = "Email";
            emailCol.DataPropertyName = "Email";
            emailCol.Width = 200;
            dgvDSHocSinh.Columns.Add(emailCol);

            dgvDSHocSinh.AllowUserToAddRows = false;

        }
        #endregion

        #region Load danh sách học sinh
        private async Task LoadStudentsAsync()
        {
            try
            {
                // Gọi API lấy danh sách user (đã phân trang hoặc nhiều trang)
                var allUserResult = await _userApi.GetAllUsersAsync();
                var allUsers = allUserResult.Users;  // LẤY DANH SÁCH USERS

                // Lấy danh sách học sinh của lớp
                var classStudents = await _classApi.GetStudentsAsync(_classId);

                // Lọc role Student
                _allStudents = allUsers.Where(u => u.Role == "Student").ToList();

                dgvDSHocSinh.Rows.Clear();

                foreach (var student in _allStudents)
                {
                    bool isInClass = classStudents.Any(s => s.UserId == student.UserId);

                    dgvDSHocSinh.Rows.Add(
                        isInClass,
                        student.FullName,
                        student.Email
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách học sinh: {ex.Message}");
            }
        }

        #endregion

        #region Lưu danh sách đã chọn
        private async void btnLuu_Click_1(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedIds = new List<int>();

                for (int i = 0; i < dgvDSHocSinh.Rows.Count; i++)
                {
                    bool isChecked = Convert.ToBoolean(dgvDSHocSinh.Rows[i].Cells[0].Value);
                    if (isChecked)
                    {
                        var student = _allStudents.First(s =>
                            s.FullName == dgvDSHocSinh.Rows[i].Cells[1].Value.ToString()
                        );

                        selectedIds.Add(student.UserId);
                    }
                }

                // Gọi API 
                await _classApi.AddStudentsToClassAsync(_classId, selectedIds);

                MessageBox.Show("Cập nhật học sinh thành công!");
                this.DialogResult = DialogResult.OK;   
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu danh sách học sinh: {ex.Message}");
            }
        }
        #endregion

        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
