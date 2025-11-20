using Academix.WinApp.Api;
using Academix.WinApp.Forms.Student;
using Academix.WinApp.Forms.Student.MyResult;
using Academix.WinApp.Models.Student;
using Academix.WinApp.Models.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Exam
{
    public partial class UC_DoExam : UserControl
    {
        private readonly ExamApiService _examApi = new();
        private StartExamResponseDto? _attempt;
        private ExamDto? _examMetadata;
        private readonly Dictionary<int, int> _selectedAnswers = new();
        private bool _isSubmitting;
        private bool _initialized;
        private TimeSpan remaining;

        public UC_DoExam()
        {
            InitializeComponent();
            PanelDoExam.FlowDirection = FlowDirection.TopDown;
            PanelDoExam.WrapContents = false;
            PanelDoExam.AutoScroll = true;
            PanelDoExam.SizeChanged += PanelDoExam_SizeChanged;
            btnNopBai.Click += btnNopBai_Click;
        }

        public void BindAttempt(StartExamResponseDto attempt, ExamDto examMetadata)
        {
            _attempt = attempt;
            _examMetadata = examMetadata;
            _selectedAnswers.Clear();
            _initialized = true;

            lblTenBaiKtra.Text = attempt.Title;
            lblTenMon.Text = examMetadata.ClassName;
            var endTime = attempt.EndTime.Kind == DateTimeKind.Utc
                ? attempt.EndTime.ToLocalTime()
                : attempt.EndTime;
            remaining = endTime - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                remaining = TimeSpan.Zero;
            }

            lblClock.Text = remaining.ToString(@"hh\:mm\:ss");
            timer1.Interval = 1000;
            timer1.Start();

            RenderQuestionCards();
            UpdateAnsweredLabel();
        }

        private void RenderQuestionCards()
        {
            if (_attempt == null)
            {
                return;
            }

            PanelDoExam.Controls.Clear();
            PanelDoExam.SuspendLayout();

            var orderedQuestions = _attempt.Questions
                .OrderBy(q => q.QuestionOrder)
                .Select((q, idx) => new { Question = q, DisplayIndex = idx + 1 });

            foreach (var item in orderedQuestions)
            {
                var card = new UC_DoExamCard
                {
                    Width = PanelDoExam.ClientSize.Width - 40,
                    Margin = new Padding(10)
                };

                item.Question.QuestionOrder = item.DisplayIndex;

                card.BindQuestion(
                    item.Question,
                    _selectedAnswers.TryGetValue(item.Question.QuestionId, out var selected) ? selected : null);

                card.OptionSelected += Card_OptionSelected;
                PanelDoExam.Controls.Add(card);
            }

            PanelDoExam.ResumeLayout();
        }

        private void PanelDoExam_SizeChanged(object? sender, EventArgs e)
        {
            var targetWidth = Math.Max(PanelDoExam.ClientSize.Width - 40, 200);
            foreach (Control control in PanelDoExam.Controls)
            {
                control.Width = targetWidth;
            }
        }

        private async void Card_OptionSelected(object? sender, OptionSelectedEventArgs e)
        {
            if (_attempt == null || _isSubmitting)
            {
                return;
            }

            _selectedAnswers[e.QuestionId] = e.SelectedOptionId;
            UpdateAnsweredLabel();

            try
            {
                var success = await _examApi.SaveAnswerAsync(_attempt.AttemptId, e.QuestionId, e.SelectedOptionId);
                if (!success)
                {
                    MessageBox.Show("Không thể lưu đáp án. Vui lòng kiểm tra kết nối và thử lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu đáp án. Vui lòng thử lại.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateAnsweredLabel()
        {
            if (_attempt == null)
            {
                lblSoCauDaTraLoi.Text = "Chưa tải được câu hỏi";
                return;
            }

            var total = _attempt.Questions.Count;
            var answered = _selectedAnswers.Count;
            lblSoCauDaTraLoi.Text = $"Đã trả lời: {answered}/{total} câu";
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                timer1.Stop();
                return;
            }

            if (remaining.TotalSeconds <= 0)
            {
                timer1.Stop();
                lblClock.Text = "00:00:00";
                await SubmitExamAsync(true);
                return;
            }

            remaining = remaining.Subtract(TimeSpan.FromSeconds(1));
            lblClock.Text = remaining.ToString(@"hh\:mm\:ss");
        }

        private async void btnNopBai_Click(object? sender, EventArgs e)
        {
            await SubmitExamAsync(false);
        }

        private async Task SubmitExamAsync(bool isAutoSubmit)
        {
            if (_attempt == null || _isSubmitting)
            {
                return;
            }

            _isSubmitting = true;
            btnNopBai.Enabled = false;

            try
            {
                var answers = _selectedAnswers
                    .Select(kvp => new ExamAnswerRequestDto
                    {
                        QuestionId = kvp.Key,
                        SelectedOptionId = kvp.Value
                    })
                    .ToList();

                var result = await _examApi.SubmitExamAsync(_attempt.AttemptId, answers);
                timer1.Stop();

                var message = isAutoSubmit
                    ? "Hết giờ! Bài kiểm tra đã được nộp tự động."
                    : "Bạn đã nộp bài thành công.";

                if (result != null)
                {
                    var percentageText = result.Percentage.HasValue
                        ? $"{Math.Round(result.Percentage.Value, 2)}%"
                        : "N/A";
                    message += $"\nĐiểm số: {result.TotalScore}/{result.TotalMarks} ({percentageText}).";
                }

                MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NavigateBackToExamList();
            }
            catch (Exception ex)
            {
                if (isAutoSubmit)
                {
                    MessageBox.Show($"Hệ thống gặp lỗi khi tự động nộp bài.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Không thể nộp bài. Vui lòng thử lại.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                _isSubmitting = false;
                btnNopBai.Enabled = true;
            }
        }

        private void NavigateBackToExamList()
        {
            var frm = Application.OpenForms["FormMainStudent"] as FormMainStudent;
            if (frm == null)
            {
                return;
            }

            frm.mainPanel.Controls.Clear();
            var examList = new UC_MyExams { Dock = DockStyle.Fill };
            frm.mainPanel.Controls.Add(examList);
        }

        private async void UC_DoExam_Load(object sender, EventArgs e)
        {
            if (_initialized || _examMetadata == null)
            {
                return;
            }

            try
            {
                var attempt = await _examApi.StartExamAsync(_examMetadata.ExamId);
                if (attempt == null)
                {
                    MessageBox.Show("Không thể bắt đầu bài kiểm tra.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NavigateBackToExamList();
                    return;
                }

                BindAttempt(attempt, _examMetadata);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải bài kiểm tra.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NavigateBackToExamList();
            }
        }
    }
}
