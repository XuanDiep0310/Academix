using Academix.WinApp.Api;
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
    public partial class UC_MyExams : UserControl
    {
        public UC_MyExams()
        {
            InitializeComponent();
        }

        private async void UC_MyExams_Load(object sender, EventArgs e)
        {
            await LoadExams();
        }

        private async Task LoadExams()
        {
            try
            {
                var examApi = new ExamApiService();

                var examsTask = examApi.GetStudentExamsAsync();
                var historyTask = examApi.GetStudentExamHistoryAsync();

                await Task.WhenAll(examsTask, historyTask);

                var allExams = examsTask.Result;
                var history = historyTask.Result;

                var attemptsByExam = history
                    .GroupBy(h => h.ExamId)
                    .ToDictionary(
                        g => g.Key,
                        g => g
                            .OrderByDescending(a => a.SubmitTime ?? a.StartTime)
                            .First());

                // Sắp xếp theo thời gian (ưu tiên StartTime, fallback CreatedAt) giảm dần
                var ordered = allExams
                    .OrderByDescending(e => e.StartTime == default ? e.CreatedAt : e.StartTime)
                    .ToList();

                flowpanelExams.Controls.Clear();

                foreach (var exam in ordered)
                {
                    var card = new UC_ExamCard();
                    card.Width = flowpanelExams.ClientSize.Width - 20;

                    attemptsByExam.TryGetValue(exam.ExamId, out var attempt);
                    card.Bind(exam, attempt);
                    card.Margin = new Padding(10);
                    flowpanelExams.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải danh sách bài kiểm tra.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void flowpanelExams_SizeChanged(object sender, EventArgs e)
        {
            ResizeCards();
        }
        private void ResizeCards()
        {
            foreach (Control c in flowpanelExams.Controls)
            {
                c.Width = flowpanelExams.ClientSize.Width - 20; // trừ margin
            }
        }
    
    }
}
