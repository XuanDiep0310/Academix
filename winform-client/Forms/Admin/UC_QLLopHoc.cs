using Academix.WinApp.Api;
using Academix.WinApp.Models.Classes;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class UC_QLLopHoc : UserControl
    {
        private readonly ClassApiService _classApiService;

        public UC_QLLopHoc()
        {
            InitializeComponent();
            _classApiService = new ClassApiService();
            InitializeDataGridView();
            Load += UC_QLLopHoc_Load;
        }
        private void InitializeDataGridView()
        {
            dgvClasses.Columns.Clear();
            dgvClasses.AutoGenerateColumns = false; // ✅ thêm dòng này
            dgvClasses.Columns.Add("ClassName", "Tên lớp");
            dgvClasses.Columns.Add("ClassCode", "Mã lớp");
            dgvClasses.Columns.Add("TeacherCount", "Giáo viên");
            dgvClasses.Columns.Add("StudentCount", "Học sinh");
            dgvClasses.Columns.Add("CreatedAt", "Ngày tạo");
            dgvClasses.Columns.Add("View", "Xem");
            dgvClasses.Columns.Add("Edit", "Sửa");
            dgvClasses.Columns.Add("Delete", "Xóa");
        }



        private async void UC_QLLopHoc_Load(object sender, EventArgs e)
        {
            await LoadClassesAsync();
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                var data = await _classApiService.GetClassesAsync();

                dgvClasses.Rows.Clear();

                foreach (var c in data.Classes)
                {
                    dgvClasses.Rows.Add(
                        c.ClassName,
                        c.ClassCode,
                        $"{c.TeacherCount}/2",
                        $"{c.StudentCount}/100",
                        c.CreatedAt.ToString("yyyy-MM-dd"),
                        "👥", "✏️", "🗑️"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu lớp: " + ex.Message);
            }
        }

        private void HandleError(string message, Exception ex)
        {
            MessageBox.Show($"{message}:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private async void btnTaoLopHoc_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormTaoLopHoc();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadClassesAsync(); //
                }
            }
            catch (Exception ex)
            {
                HandleError("Lỗi khi mở form thêm lớp học", ex);
            }
        }

    }
}
