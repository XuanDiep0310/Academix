using Academix.WinApp.Api;
using Academix.WinApp.Forms.Student;
using Academix.WinApp.Forms.Student.Exam;
using Academix.WinApp.Models.Student;
using Academix.WinApp.Models.Teacher;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.MyResult
{
    public partial class UC_ExamCard : UserControl
    {
        private ExamDto? _exam;
        private StudentExamResultDto? _attemptResult;
        private bool _isLoading;

        public UC_ExamCard()
        {
            InitializeComponent();
        }

        public void Bind(ExamDto exam, StudentExamResultDto? attemptResult = null)
        {
            _exam = exam;
            _attemptResult = attemptResult;

            lblTenBaiKiemTra.Text = exam.Title;
            lblLopHoc.Text = exam.ClassName;
            //lblMonHoc.Text = exam.ClassName;
            lblThoiLuong.Text = $"{exam.Duration} phút";
            lblThoiGianBatDau.Text = exam.StartTime.ToString("HH:mm dd/MM/yyyy");
            lblThoiGianKetThuc.Text = exam.EndTime.ToString("HH:mm dd/MM/yyyy");

            if (_attemptResult != null)
            {
                lblTrangThai.Text = "Đã hoàn thành";
                lblTrangThai.BackColor = Color.MediumSeaGreen;
                lblTrangThai.ForeColor = Color.White;

                var submittedAt = _attemptResult.SubmitTime?.ToLocalTime() ?? _attemptResult.StartTime.ToLocalTime();
                var percentageText = _attemptResult.Percentage.HasValue
                    ? $"{Math.Round(_attemptResult.Percentage.Value, 2)}%"
                    : "N/A";

                lblTrangThaiDangMo.Text = $"Bạn đã nộp lúc {submittedAt:HH:mm dd/MM/yyyy}. Điểm: {_attemptResult.TotalScore}/{_attemptResult.TotalMarks} ({percentageText}).";
                lblTrangThaiDangMo.ForeColor = Color.DimGray;
                BackColor = Color.FromArgb(240, 250, 240);
                btnBatDauLamBai.Visible = false;
                btnBatDauLamBai.Enabled = false;
                return;
            }

            var now = DateTime.Now;
            var isOpen = exam.IsPublished
                && now >= exam.StartTime
                && now <= exam.EndTime;

            // Cập nhật giao diện theo trạng thái
            if (isOpen)
            {
                // Bài đang mở - màu xanh, nổi bật
                lblTrangThai.Text = "Đang mở";
                lblTrangThai.BackColor = Color.SkyBlue;
                lblTrangThai.ForeColor = Color.DarkBlue;
                lblTrangThaiDangMo.Text = "Bài kiểm tra đang mở. Bạn có thể làm bài ngay bây giờ!";
                lblTrangThaiDangMo.ForeColor = Color.LimeGreen;
                BackColor = Color.White;
                btnBatDauLamBai.Visible = true;
                btnBatDauLamBai.Enabled = true;
            }
            else
            {
                // Bài đã đóng - màu xám, mờ
                lblTrangThai.Text = "Đã đóng";
                lblTrangThai.BackColor = Color.LightGray;
                lblTrangThai.ForeColor = Color.DarkGray;

                string reason = "Bài kiểm tra đã kết thúc.";
                if (!exam.IsPublished)
                    reason = "Bài kiểm tra chưa được mở.";
                else if (now < exam.StartTime)
                    reason = "Bài kiểm tra chưa đến thời gian bắt đầu.";
                else if (now > exam.EndTime)
                    reason = "Bài kiểm tra đã hết thời gian.";

                lblTrangThaiDangMo.Text = reason;
                lblTrangThaiDangMo.ForeColor = Color.Gray;
                BackColor = Color.FromArgb(245, 245, 245); // Nền xám nhạt
                btnBatDauLamBai.Visible = false;
                btnBatDauLamBai.Enabled = false;
            }
        }

        private async void btnBatDauLamBai_Click(object sender, EventArgs e)
        {
            if (_exam == null || _isLoading)
                return;

            if (_attemptResult != null)
            {
                MessageBox.Show("Bạn đã hoàn thành bài kiểm tra này và không thể làm lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var frm = Application.OpenForms["FormMainStudent"] as FormMainStudent;
            if (frm == null)
            {
                MessageBox.Show("FormMainStudent chưa mở!");
                return;
            }

            try
            {
                _isLoading = true;
                btnBatDauLamBai.Enabled = false;
                var examApi = new ExamApiService();
                var attempt = await examApi.StartExamAsync(_exam.ExamId);

                if (attempt == null)
                {
                    MessageBox.Show("Không thể bắt đầu bài kiểm tra. Vui lòng thử lại sau.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                frm.mainPanel.Controls.Clear();
                var uc = new UC_DoExam();
                uc.Dock = DockStyle.Fill;
                uc.BindAttempt(attempt, _exam);
                frm.mainPanel.Controls.Add(uc);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể bắt đầu bài kiểm tra.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
                btnBatDauLamBai.Enabled = true;
            }
        }
    }
}
