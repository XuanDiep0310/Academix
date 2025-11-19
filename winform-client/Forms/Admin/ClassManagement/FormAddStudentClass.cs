using Academix.WinApp.Api;
using Academix.WinApp.Models.Classes;
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
        private List<ClassMember> _allStudents;

        public FormAddStudentClass(int classId)
        {
            InitializeComponent();
            _classId = classId;
            //_classApi = new ClassApiService("YOUR_BEARER_TOKEN"); // gán token

            this.Load += FormAddStudentClass_Load;
        }

        private async void FormAddStudentClass_Load(object sender, EventArgs e)
        {
            //await LoadStudentsAsync();
        }

        //private async Task LoadStudentsAsync()
        //{
        //    try
        //    {
        //        // Lấy danh sách tất cả học sinh (từ API hoặc giả lập)
        //        _allStudents = await _classApi.GetAllStudentsAsync(); // API trả về tất cả học sinh chưa vào lớp

        //        dgvStudents.Rows.Clear();
        //        dgvStudents.Columns.Clear();

        //        dgvStudents.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "Chọn" });
        //        dgvStudents.Columns.Add("UserId", "ID");
        //        dgvStudents.Columns.Add("FullName", "Họ và tên");
        //        dgvStudents.Columns.Add("Email", "Email");

        //        foreach (var s in _allStudents)
        //        {
        //            dgvStudents.Rows.Add(false, s.UserId, s.FullName, s.Email);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi load danh sách học sinh: " + ex.Message);
        //    }
        //}

        //private void btnHuy_Click(object sender, EventArgs e)
        //{
        //    this.DialogResult = DialogResult.Cancel;
        //    this.Close();
        //}

        //private async void btnLuu_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Lấy danh sách UserId đã chọn
        //        var selectedIds = new List<int>();
        //        for (int i = 0; i < dgvStudents.Rows.Count; i++)
        //        {
        //            bool isChecked = Convert.ToBoolean(dgvStudents.Rows[i].Cells[0].Value);
        //            if (isChecked)
        //            {
        //                selectedIds.Add(Convert.ToInt32(dgvStudents.Rows[i].Cells["UserId"].Value));
        //            }
        //        }

        //        if (!selectedIds.Any())
        //        {
        //            MessageBox.Show("Vui lòng chọn ít nhất 1 học sinh!");
        //            return;
        //        }

        //        // Gọi API thêm học sinh vào lớp
        //        var result = await _classApi.AddStudentsAsync(_classId, selectedIds);

        //        if (result.Success)
        //        {
        //            MessageBox.Show("Thêm học sinh thành công!");
        //            this.DialogResult = DialogResult.OK;
        //            this.Close();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Thêm học sinh thất bại: " + result.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi thêm học sinh: " + ex.Message);
        //    }
        //}
    }
}
