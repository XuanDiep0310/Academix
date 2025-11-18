using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Class;
using Academix.WinApp.Models.Classes;
using Academix.WinApp.Models.Teacher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student
{
    public partial class UC_MyClassCard : UserControl
    {

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ClassId { get; set; }

        public UC_MyClassCard()
        {
        }

        public UC_MyClassCard(int classId, string className, string classCode)
        {
            InitializeComponent();
            ClassId = classId;
            lblTenLop.Text = className;
            lblMaLop.Text = classCode;
            lblTenGiaoVien.Text = "Đang tải...";
            lblSoTaiLieu.Text = "-";
            lblSoBaiKiemTra.Text = "-";

            // chạy bất đồng bộ sau khi control đã load
            this.Load += async (s, e) => await LoadSummaryAsync();
        }

        public async Task LoadSummaryAsync()
        {
            try
            {
                var classApi = new ClassApiService();
                var materialApi = new MaterialApiService();
                var examApi = new ExamApiService();

                // Lấy thông tin giáo viên
                ClassDetailDto? detail = await classApi.GetClassDetailAsync(ClassId);
                int teacherCount = 0;

                if (detail != null)
                {
                    // ưu tiên TeacherCount nếu API cung cấp
                    teacherCount = detail.TeacherCount > 0
                        ? detail.TeacherCount
                        : (detail.Teachers?.Count ?? 0);

                    if (detail.Teachers != null && detail.Teachers.Count > 0)
                    {
                        var teachers = detail.Teachers
                            .Take(2)
                            .Select(t => t.FullName)
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .ToList();

                        if (teachers.Count > 0)
                        {
                            var countText = teacherCount > 0 ? $" ({teacherCount} GV)" : string.Empty;
                            lblTenGiaoVien.Text = string.Join(", ", teachers) + countText;
                        }
                        else
                        {
                            lblTenGiaoVien.Text = "Chưa có giáo viên";
                        }
                    }
                    else
                    {
                        lblTenGiaoVien.Text = "Chưa có giáo viên";
                    }
                }
                else
                {
                    lblTenGiaoVien.Text = "Chưa có giáo viên";
                }

                // Đếm số tài liệu
                var materialPaged = await materialApi.GetMaterialsPagedAsync(
                    ClassId,
                    typeFilter: null,
                    page: 1,
                    pageSize: 1
                );
                lblSoTaiLieu.Text = materialPaged.TotalCount.ToString();

                // Đếm số bài kiểm tra
                var examsPaged = await examApi.GetExamsByClassAsync(
                    ClassId,
                    isPublished: null,
                    page: 1,
                    pageSize: 1
                );
                lblSoBaiKiemTra.Text = examsPaged.TotalCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải thông tin lớp (GV/tài liệu/bài kiểm tra).\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                if (string.IsNullOrWhiteSpace(lblTenGiaoVien.Text) || lblTenGiaoVien.Text == "Đang tải...")
                    lblTenGiaoVien.Text = "Không tải được";

                if (lblSoTaiLieu.Text == "-")
                    lblSoTaiLieu.Text = "0";

                if (lblSoBaiKiemTra.Text == "-")
                    lblSoBaiKiemTra.Text = "0";
            }
        }
    }
}
