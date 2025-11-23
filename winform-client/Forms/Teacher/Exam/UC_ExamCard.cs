using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Exam;
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

namespace Academix.WinApp.Forms.Teacher
{
    public partial class UC_ExamCard : UserControl
    {
        public event Func<Task>? OnUpdated; // callback reload

        private ExamResponseDto _exam;

        public UC_ExamCard(ExamResponseDto exam)
        {
            InitializeComponent();
            _exam = exam;

            // Bind dữ liệu bài kiểm tra lên UI
            lblKiemTra.Text = _exam.Title;
            lblLop.Text = _exam.ClassName;
            lblTrangThai.Text = _exam.IsPublished ? "Đã công bố" : "Bản nháp";
            

            // Nếu đã công bố thì không cho xóa, nhưng vẫn cho sửa
            if (_exam.IsPublished)
            {
                lblTrangThai.ForeColor = Color.LightGreen;
                btnXoa.Visible = false; // Không cho xóa bài đã công bố
                btnCongBo.Visible = false; // Không cho công bố lại
                // Vẫn cho phép sửa và xem chi tiết
            }

            lblThoiGian.Text = $"Thời lượng: {_exam.Duration} phút";
            lblSoLuong.Text = $"Số câu hỏi: {_exam.QuestionCount}";
            lblBatDau.Text = $"Bắt đầu: {_exam.StartTime}";
            lblKetThuc.Text = $"Kết thúc: {_exam.EndTime}";
        }

        private async void btnSua_Click(object sender, EventArgs e)
        {
            // Cho phép sửa cả bài đã công bố
            Form_AddUpdateExam frm = new Form_AddUpdateExam(_exam.ClassId, _exam.ExamId);

            // Gọi callback khi form lưu
            frm.OnSaved += async () =>
            {
                if (OnUpdated != null)
                    await OnUpdated();
            };

            frm.Show();
        }

        private async void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            // Mở form xem chi tiết bài kiểm tra
            using var detailForm = new Form_ExamDetail(_exam.ClassId, _exam.ExamId);
            detailForm.ShowDialog();
        }

        private async void btnCongBo_Click(object sender, EventArgs e)
        {
            // Công bố bài kiểm tra
            if (_exam.IsPublished)
            {
                MessageBox.Show("Bài kiểm tra đã được công bố trước đó.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn công bố bài kiểm tra này không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            var api = new ExamApiService();
            var resp = await api.PublishExamAsync(_exam.ClassId, _exam.ExamId);

            if (!resp.Success)
            {
                MessageBox.Show(resp.Message ?? "Không thể công bố bài kiểm tra.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cập nhật trạng thái local
            _exam.IsPublished = true;


            // Ẩn các nút sau khi công bố (chỉ ẩn xóa và công bố, vẫn giữ sửa)
            btnXoa.Visible = false; 
            btnCongBo.Visible = false;


            MessageBox.Show("Công bố bài kiểm tra thành công.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (OnUpdated != null)
                await OnUpdated();
        }


        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            // Xóa bài kiểm tra (chỉ cho phép khi chưa công bố)
            if (_exam.IsPublished)
            {
                MessageBox.Show("Không thể xóa bài kiểm tra đã công bố.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa bài kiểm tra này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            var api = new ExamApiService();
            var resp = await api.DeleteExamAsync(_exam.ClassId, _exam.ExamId);
            if (!resp.Success)
            {
                MessageBox.Show(resp.Message ?? "Không thể xóa bài kiểm tra.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Xóa bài kiểm tra thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (OnUpdated != null)
                await OnUpdated();
        }
    }

}
