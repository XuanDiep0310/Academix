using Academix.WinApp.Api;
using Academix.WinApp.Forms.Admin.ClassManagement;
using Academix.WinApp.Models.Classes;
using Academix.WinApp.Models.Classes.Responses;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class UC_QLLopHoc : UserControl
    {
        private readonly ClassApiService _classApiService;
        private List<ClassDto> _currentClasses;

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
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;

        private async Task LoadClassesAsync()
        {
            try
            {
                var data = await _classApiService.GetClassesAsync(
                    page: _currentPage,
                    pageSize: _pageSize,
                    sortBy: "CreatedAt",
                    sortOrder: "desc"
                );

                // Lưu danh sách lớp hiện tại để phục vụ tìm kiếm
                _currentClasses = data.Classes ?? new List<ClassDto>();

                // Áp dụng filter theo ô tìm kiếm (nếu có)
                ApplySearchFilter(timkiem.Text);

                // Gán tổng số trang từ API
                _totalPages = data.TotalPages > 0 ? data.TotalPages : 1;

                // Hiển thị thông tin trang
                lblPageInfo.Text = $"Trang {_currentPage}/{_totalPages}";

                UpdatePaginationButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu lớp: " + ex.Message);
            }
        }
        private void UpdatePaginationButtons()
        {
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }





        private async void dgvClasses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                // Lấy ClassId từ Tag của row
                var row = dgvClasses.Rows[e.RowIndex];
                if (row.Tag == null)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu lớp!");
                    return;
                }

                int classId = (int)row.Tag;

                int colView = dgvClasses.Columns["View"].Index;
                int colEdit = dgvClasses.Columns["Edit"].Index;
                int colDelete = dgvClasses.Columns["Delete"].Index;

                // VIEW: mở danh sách thành viên
                if (e.ColumnIndex == colView)
                {
                    string className = row.Cells["ClassName"].Value?.ToString() ?? "Không xác định";
                    using var form = new FormThanhVienLop(classId, className);
                    form.ShowDialog();
                }
                // EDIT
                else if (e.ColumnIndex == colEdit)
                {
                    using var form = new FormSuaLop(classId);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        await LoadClassesAsync();
                    }
                }
                // DELETE
                else if (e.ColumnIndex == colDelete)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa lớp ID {classId}?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            await _classApiService.DeleteClassAsync(classId);
                            MessageBox.Show("Xóa lớp thành công!");
                            await LoadClassesAsync();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể xóa lớp: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý lớp: " + ex.Message);
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

        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadClassesAsync();
            }
        }


        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadClassesAsync();
            }
        }

        private void timkiem_TextChanged(object sender, EventArgs e)
        {
            ApplySearchFilter(timkiem.Text);
        }

        private void btnTimKim1_Click(object sender, EventArgs e)
        {
            ApplySearchFilter(timkiem.Text);
        }

        /// <summary>
        /// Lọc danh sách lớp theo từ khóa (tên lớp, mã lớp, người tạo)
        /// </summary>
        private void ApplySearchFilter(string keyword)
        {
            if (_currentClasses == null)
            {
                dgvClasses.Rows.Clear();
                return;
            }

            keyword = keyword?.Trim() ?? string.Empty;

            IEnumerable<ClassDto> source = _currentClasses;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lower = keyword.ToLower();

                source = _currentClasses.Where(c =>
                    (!string.IsNullOrEmpty(c.ClassName) && c.ClassName.ToLower().Contains(lower)) ||
                    (!string.IsNullOrEmpty(c.ClassCode) && c.ClassCode.ToLower().Contains(lower)) ||
                    (!string.IsNullOrEmpty(c.CreatedByName) && c.CreatedByName.ToLower().Contains(lower)));
            }

            dgvClasses.Rows.Clear();

            foreach (var c in source)
            {
                int rowIndex = dgvClasses.Rows.Add(
                    c.ClassName,
                    c.ClassCode,
                    $"{c.TeacherCount}/2",
                    $"{c.StudentCount}/100",
                    c.CreatedAt.ToString("yyyy-MM-dd"),
                    "👥", "✏️", "🗑️"
                );

                dgvClasses.Rows[rowIndex].Tag = c.ClassId;
            }
        }
    }
}
