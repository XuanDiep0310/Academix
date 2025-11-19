using Academix.WinApp.Api;
using Academix.WinApp.Models.Classes;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    public partial class FormAddTeacherClass : Form
    {
        private readonly int _classId;
        private readonly ClassApiService _classApi;
        private readonly UserApi _userApi;
        private List<UserData> _allTeachers; // danh sách tất cả giáo viên

        public FormAddTeacherClass(int classId)
        {
            InitializeComponent();
            _classId = classId;
            _classApi = new ClassApiService();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));

            this.Load += FormAddTeacherClass_Load;
        }

        private async void FormAddTeacherClass_Load(object sender, EventArgs e)
        {
            InitializeTeacherGrid();
            await LoadTeachersAsync();
        }

        #region Khởi tạo DataGridView
        private void InitializeTeacherGrid()
        {
            dgvDSGiaoVien.AutoGenerateColumns = false;
            dgvDSGiaoVien.Columns.Clear();

            // Checkbox column
            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.HeaderText = "Chọn";
            chkCol.Width = 50;
            dgvDSGiaoVien.Columns.Add(chkCol);

            // Name column
            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.HeaderText = "Tên giáo viên";
            nameCol.DataPropertyName = "FullName";
            nameCol.Width = 200;
            dgvDSGiaoVien.Columns.Add(nameCol);

            // Email column
            DataGridViewTextBoxColumn emailCol = new DataGridViewTextBoxColumn();
            emailCol.HeaderText = "Email";
            emailCol.DataPropertyName = "Email";
            emailCol.Width = 200;
            dgvDSGiaoVien.Columns.Add(emailCol);

            dgvDSGiaoVien.AllowUserToAddRows = false;

        }
        #endregion

        #region Load danh sách giáo viên
        private async Task LoadTeachersAsync()
        {
            try
            {
                // Lấy dữ liệu từ API
                var allUsersResult = await _userApi.GetAllUsersAsync();
                var allUsers = allUsersResult.Users;  // Lấy danh sách user

                // Lấy danh sách giáo viên trong lớp
                var classTeachers = await _classApi.GetTeachersAsync(_classId);

                // Lọc role Teacher
                _allTeachers = allUsers
                    .Where(u => u.Role == "Teacher")
                    .ToList();

                dgvDSGiaoVien.Rows.Clear();

                foreach (var teacher in _allTeachers)
                {
                    bool isInClass = classTeachers.Any(ct => ct.UserId == teacher.UserId);

                    dgvDSGiaoVien.Rows.Add(
                        isInClass,
                        teacher.FullName,
                        teacher.Email
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách giáo viên: {ex.Message}");
            }
        }

        #endregion

        #region Lưu danh sách giáo viên đã chọn
        private async void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedTeacherIds = new List<int>();

                for (int i = 0; i < dgvDSGiaoVien.Rows.Count; i++)
                {
                    bool isChecked = Convert.ToBoolean(dgvDSGiaoVien.Rows[i].Cells[0].Value);
                    if (isChecked)
                    {
                        var teacher = _allTeachers.First(t =>
                            t.FullName == dgvDSGiaoVien.Rows[i].Cells[1].Value.ToString()
                        );

                        selectedTeacherIds.Add(teacher.UserId);
                    }
                }

                // 🚫 Chỉ cho chọn tối đa 2 giáo viên
                if (selectedTeacherIds.Count > 2)
                {
                    MessageBox.Show("Chỉ được chọn tối đa 2 giáo viên cho lớp!",
                                    "Cảnh báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Gọi API để cập nhật giáo viên cho lớp
                await _classApi.AddTeachersToClassAsync(_classId, selectedTeacherIds);

                MessageBox.Show("Cập nhật giáo viên thành công!");
                this.DialogResult = DialogResult.OK;   
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu danh sách giáo viên: {ex.Message}");
            }
        }

        #endregion

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
