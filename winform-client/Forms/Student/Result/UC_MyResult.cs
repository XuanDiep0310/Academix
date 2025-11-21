using Academix.WinApp.Api;
using Academix.WinApp.Forms.Student.Result;
using Academix.WinApp.Models.Student;
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

namespace Academix.WinApp.Forms.Student.MyResult
{
    public partial class UC_MyResult : UserControl
    {
        public UC_MyResult()
        {
            InitializeComponent();
        }

        private async void UC_MyResult_Load(object sender, EventArgs e)
        {
            await LoadResults();
        }

        private async Task LoadResults()
        {
            try
            {
                var examApi = new ExamApiService();

                // Load cả danh sách exams để lấy ClassName
                var examsTask = examApi.GetStudentExamsAsync();
                var historyTask = examApi.GetStudentExamHistoryAsync();

                await Task.WhenAll(examsTask, historyTask);

                var allExams = examsTask.Result;
                var history = historyTask.Result;

                // Tạo dictionary để map ExamId -> ClassName
                var examClassMap = allExams
                    .GroupBy(e => e.ExamId)
                    .ToDictionary(g => g.Key, g => g.First().ClassName);

                // Map ClassName vào history results
                foreach (var result in history)
                {
                    if (examClassMap.TryGetValue(result.ExamId, out var className))
                    {
                        result.ClassName = className;
                    }
                }

                var ordered = history
                    .OrderByDescending(r => r.StartTime)
                    .ToList();

                // Tính toán và cập nhật thống kê
                UpdateStatistics(ordered);

                flowpanelResult.Controls.Clear();

                if (!ordered.Any())
                {
                    var emptyLabel = new Label
                    {
                        Text = "Bạn chưa có bài kiểm tra nào.",
                        AutoSize = true,
                        ForeColor = Color.DimGray,
                        Font = new Font("Segoe UI", 10, FontStyle.Italic),
                        Margin = new Padding(10)
                    };
                    flowpanelResult.Controls.Add(emptyLabel);
                    return;
                }

                foreach (var result in ordered)
                {
                    var card = new UCResultCard();
                    card.Bind(result);
                    flowpanelResult.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải kết quả bài kiểm tra.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void UpdateStatistics(List<StudentExamResultDto> results)
        {
            if (results == null || !results.Any())
            {
                // Reset về giá trị mặc định
                lblDiemtrungBinh.Text = "0/10";
                lblTongSoBaiDaLam.Text = "0";
                lblTyLeChinhXac.Text = "0%";
                guna2ProgressBar1.Value = 0;
                guna2ProgressBar2.Value = 0;
                return;
            }

            // Tính tổng số bài đã làm (chỉ tính các bài đã nộp)
            var completedExams = results.Where(r => r.SubmitTime.HasValue).ToList();
            int totalExams = completedExams.Count;
            lblTongSoBaiDaLam.Text = totalExams.ToString();

            if (totalExams == 0)
            {
                lblDiemtrungBinh.Text = "0/10";
                lblTyLeChinhXac.Text = "0%";
                guna2ProgressBar1.Value = 0;
                guna2ProgressBar2.Value = 0;
                return;
            }

            // Tính điểm trung bình
            double averageScore = completedExams.Average(r => r.TotalScore);
            double averageTotalMarks = completedExams.Average(r => r.TotalMarks);
            lblDiemtrungBinh.Text = $"{averageScore:F1}/{averageTotalMarks:F1}";
            
            // Cập nhật progress bar cho điểm trung bình (tính theo phần trăm)
            double averagePercentage = averageTotalMarks > 0 ? (averageScore / averageTotalMarks) * 100 : 0;
            guna2ProgressBar1.Value = (int)Math.Round(averagePercentage);
            guna2ProgressBar1.Text = $"{averagePercentage:F1}%";

            // Tính tỷ lệ chính xác (trung bình của tỷ lệ đúng/tổng câu hỏi)
            double averageAccuracy = completedExams.Average(r => 
                r.TotalQuestions > 0 ? (double)r.CorrectAnswers / r.TotalQuestions * 100 : 0);
            lblTyLeChinhXac.Text = $"{averageAccuracy:F0}%";
            
            // Cập nhật progress bar cho tỷ lệ chính xác
            guna2ProgressBar2.Value = (int)Math.Round(averageAccuracy);
            guna2ProgressBar2.Text = $"{averageAccuracy:F1}%";
        }

        private void guna2Panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel6_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
