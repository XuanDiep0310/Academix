using Academix.WinApp.Api;
using Academix.WinApp.Forms.Admin.ClassManagement;
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
            dgvClasses.CellClick += dgvClasses_CellClick;

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

        private async void dgvClasses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Lấy mã lớp để truyền sang form
            string classCode = dgvClasses.Rows[e.RowIndex].Cells["ClassCode"].Value?.ToString();
            if (string.IsNullOrEmpty(classCode))
            {
                MessageBox.Show("Không tìm thấy mã lớp!");
                return;
            }

            int colView = dgvClasses.Columns["View"].Index;
            int colEdit = dgvClasses.Columns["Edit"].Index;
            int colDelete = dgvClasses.Columns["Delete"].Index;

            // VIEW: mở danh sách thành viên
            if (e.ColumnIndex == colView)
            {
                using var form = new FormThanhVienLop(classCode);
                form.ShowDialog();
            }

            // EDIT: mở form sửa lớp
            else if (e.ColumnIndex == colEdit)
            {
                //using var form = new FormSuaLop(classCode);
                //if (form.ShowDialog() == DialogResult.OK)
                //{
                //    await LoadClassesAsync(); // load lại danh sách
                //}
            }

            // DELETE: xóa lớp
            else if (e.ColumnIndex == colDelete)
            {
                //var confirm = MessageBox.Show(
                //    $"Bạn có chắc muốn xóa lớp '{classCode}'?",
                //    "Xác nhận xóa",
                //    MessageBoxButtons.YesNo,
                //    MessageBoxIcon.Warning);

                //if (confirm == DialogResult.Yes)
                //{
                //    try
                //    {
                //        await _classApiService.DeleteClassAsync(classCode);
                //        MessageBox.Show("Xóa lớp thành công!");
                //        await LoadClassesAsync();
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show("Không thể xóa lớp: " + ex.Message);
                //    }
                //}
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
