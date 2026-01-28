using Academix.WinApp.Api;
using Academix.WinApp.Forms.Student;
using Academix.WinApp.Forms.Student.MyResult;
using Academix.WinApp.Models.Student;
using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Utils;
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

        private DateTime _lastClickTime = DateTime.MinValue;
        private int _rapidClickCount = 0;

        private bool _antiCheatHooked;

        private bool _isHandlingDeactivate;
        private bool _answerLocked;
        private DateTime _lockUntil;
        private bool _isFullScreen;
        private DateTime _lastDeactivateWarning = DateTime.MinValue;
        private System.Windows.Forms.Timer? _unlockTimer;
        private bool _isShowingWarning; // Flag để tránh trigger Deactivate khi đang hiện MessageBox
        private FormWindowState _prevWindowState;
        private FormBorderStyle _prevBorderStyle;
        private bool _prevControlBox;



        private void HookAntiCheat()
        {
            if (_antiCheatHooked || ParentForm == null) return;

            ParentForm.Activated += ParentForm_Activated;
            ParentForm.Deactivate += ParentForm_Deactivate;
            _antiCheatHooked = true;
        }

        private void UnhookAntiCheat()
        {
            if (!_antiCheatHooked || ParentForm == null) return;

            ParentForm.Activated -= ParentForm_Activated;
            ParentForm.Deactivate -= ParentForm_Deactivate;
            _antiCheatHooked = false;
        }

        private void EnterFullScreen()
        {
            if (_isFullScreen || ParentForm == null) return;

            var form = ParentForm;

            _prevWindowState = form.WindowState;
            _prevBorderStyle = form.FormBorderStyle;
            _prevControlBox = form.ControlBox;

            form.ControlBox = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;

            _isFullScreen = true;
        }

        private void ExitFullScreen()
        {
            if (!_isFullScreen || ParentForm == null) return;

            var form = ParentForm;

            form.ControlBox = _prevControlBox;
            form.FormBorderStyle = _prevBorderStyle;
            form.WindowState = _prevWindowState;

            _isFullScreen = false;
        }

        private void ParentForm_Activated(object? sender, EventArgs e)
        {
            _rapidClickCount = 0;
        }

        private async void ParentForm_Deactivate(object? sender, EventArgs e)
        {
            // Không cảnh báo nếu đang submit, đang hiện warning khác, hoặc đã xử lý rồi
            if (_isHandlingDeactivate || _isSubmitting || _isShowingWarning) return;
            _isHandlingDeactivate = true;

            try
            {
                await Task.Delay(200); // cho Windows ổn định focus

                // Check lại sau delay
                if (_isSubmitting || _isShowingWarning || ParentForm == null)
                {
                    return;
                }

                // Cooldown 3 giây - không hiện cảnh báo liên tục
                var now = DateTime.Now;
                if ((now - _lastDeactivateWarning).TotalSeconds >= 3)
                {
                    _lastDeactivateWarning = now;
                    _isShowingWarning = true;
                    try
                    {
                        MessageBox.Show(
                            "Không được rời khỏi màn hình làm bài!",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        _isShowingWarning = false;
                    }
                }

                // Kéo form về foreground
                if (ParentForm != null)
                {
                    ParentForm.TopMost = true;
                    ParentForm.TopMost = false;
                }
            }
            finally
            {
                _isHandlingDeactivate = false;
            }
        }




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

            var frm = FindForm() as FormMainStudent;
            frm?.DisableNavigation();


            //khóa bàn phím chuyển app
            KeyboardLocker.Lock();

            HookAntiCheat();
            EnterFullScreen();

            // trộn đềđề
            ExamShuffleHelper.ShuffleExam(_attempt);


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
            // Kiểm tra rapid click - nếu bị chặn thì return ngay
            if (!DetectRapidClick())
                return;

            if (_answerLocked || _attempt == null || _isSubmitting)
                return;

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

                KeyboardLocker.Unlock();
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

                // Gỡ bỏ anti-cheat hooks TRƯỚC khi hiện MessageBox (tránh trigger Deactivate)
                UnhookAntiCheat();

                // Dọn dẹp unlock timer nếu có
                _unlockTimer?.Stop();
                _unlockTimer?.Dispose();
                _unlockTimer = null;

                // Mở khóa bàn phím
                KeyboardLocker.Unlock();

                // Thoát fullscreen và bật lại navigation
                ExitFullScreen();
                var frm = FindForm() as FormMainStudent;
                frm?.EnableNavigation();


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

        /// <summary>
        /// Kiểm tra click nhanh bất thường. 
        /// Return true nếu cho phép tiếp tục, false nếu bị chặn.
        /// </summary>
        private bool DetectRapidClick()
        {
            // Nếu đang bị khóa
            if (_answerLocked)
            {
                var remain = (_lockUntil - DateTime.Now).TotalSeconds;
                if (remain > 0)
                {
                    _isShowingWarning = true;
                    MessageBox.Show(
                        $"Bạn đang bị khóa thao tác trong {Math.Ceiling(remain)} giây!",
                        "Bị khóa thao tác",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    _isShowingWarning = false;
                    return false; // Chặn
                }
                else
                {
                    // Hết thời gian khóa -> mở lại
                    _answerLocked = false;
                    EnableAnswerControls(true);
                }
            }

            var now = DateTime.Now;

            // Nếu click trong vòng 10 giây so với lần trước (rapid click)
            if ((now - _lastClickTime).TotalSeconds < 10)
            {
                _rapidClickCount++;

                // Nếu click nhanh >= 5 lần liên tiếp -> khóa 10 giây
                if (_rapidClickCount >= 5)
                {
                    LockAnswerForSeconds(10);
                    _rapidClickCount = 0;
                    return false; // Chặn
                }
            }
            else
            {
                // Reset đếm nếu click chậm lại
                _rapidClickCount = 0;
            }

            _lastClickTime = now;
            return true; // Cho phép tiếp tục
        }

        private void LockAnswerForSeconds(int seconds)
        {
            _answerLocked = true;
            _lockUntil = DateTime.Now.AddSeconds(seconds);

            EnableAnswerControls(false);

            // Tạo Timer để tự động unlock sau X giây
            _unlockTimer?.Stop();
            _unlockTimer?.Dispose();
            _unlockTimer = new System.Windows.Forms.Timer();
            _unlockTimer.Interval = seconds * 1000;
            _unlockTimer.Tick += (s, e) =>
            {
                _unlockTimer?.Stop();
                _unlockTimer?.Dispose();
                _unlockTimer = null;

                _answerLocked = false;
                EnableAnswerControls(true);
            };
            _unlockTimer.Start();

            // Set flag để tránh trigger Deactivate khi hiện MessageBox
            _isShowingWarning = true;
            MessageBox.Show(
                $"Phát hiện thao tác bất thường!\nBạn bị khóa chọn đáp án trong {seconds} giây.",
                "Cảnh báo gian lận",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _isShowingWarning = false;
        }

        private void EnableAnswerControls(bool enabled)
        {
            foreach (Control c in PanelDoExam.Controls)
            {
                if (c is UC_DoExamCard card)
                {
                    card.SetEnabled(enabled);
                }
            }
        }




    }
}
