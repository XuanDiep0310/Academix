using Academix.WinApp.Api;
using Academix.WinApp.Models.Teacher;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher.Exam
{
    public partial class Form_ExamDetail : Form
    {
        private readonly int _classId;
        private readonly int _examId;
        private readonly ExamApiService _api = new ExamApiService();
        
        private Guna.UI2.WinForms.Guna2Panel topPanel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInfo;
        private Panel mainPanel;
        private FlowLayoutPanel flowPanelQuestions;

        public Form_ExamDetail(int classId, int examId)
        {
            _classId = classId;
            _examId = examId;
            InitializeComponent();
            LoadExamDetailAsync();
        }

        private void InitializeComponent()
        {
            topPanel = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblInfo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            mainPanel = new Panel();
            flowPanelQuestions = new FlowLayoutPanel();
            
            topPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            SuspendLayout();
            
            // topPanel
            topPanel.BackColor = Color.FromArgb(240, 248, 255);
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblInfo);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(20);
            topPanel.Size = new Size(1000, 120);
            topPanel.TabIndex = 0;
            
            // lblTitle
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(70, 130, 180);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(960, 35);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Đang tải...";
            
            // lblInfo
            lblInfo.BackColor = Color.Transparent;
            lblInfo.Font = new Font("Segoe UI", 12F);
            lblInfo.ForeColor = Color.FromArgb(60, 60, 60);
            lblInfo.Location = new Point(20, 65);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(960, 25);
            lblInfo.TabIndex = 1;
            lblInfo.Text = "Đang tải thông tin...";

            // flowPanelQuestions
            flowPanelQuestions.AutoScroll = true;
            flowPanelQuestions.BackColor = Color.White;
            flowPanelQuestions.Dock = DockStyle.Fill;
            flowPanelQuestions.FlowDirection = FlowDirection.LeftToRight;  // Dàn ngang
            flowPanelQuestions.WrapContents = true;                        // Xuống dòng
            flowPanelQuestions.Padding = new Padding(20);
            flowPanelQuestions.Resize += FlowPanelQuestions_Resize;        // Auto resize children


            // mainPanel
            mainPanel.Controls.Add(flowPanelQuestions);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 120);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1000, 480);
            mainPanel.TabIndex = 1;
            
            // Form
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(mainPanel);
            Controls.Add(topPanel);
            MinimumSize = new Size(800, 500);
            Name = "Form_ExamDetail";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chi tiết bài kiểm tra";
            
            topPanel.ResumeLayout(false);
            mainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void FlowPanelQuestions_Resize(object sender, EventArgs e)
        {
            foreach (Control c in flowPanelQuestions.Controls)
            {
                c.Width = flowPanelQuestions.ClientSize.Width - 60;
            }
        }

        private async void LoadExamDetailAsync()
        {
            try
            {
                var examResponse = await _api.GetExamByIdAsync(_classId, _examId);
                if (!examResponse.Success || examResponse.Data == null)
                {
                    MessageBox.Show("Không thể tải thông tin bài kiểm tra.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }

                var exam = examResponse.Data;
                
                // Load câu hỏi
                var questionsResponse = await _api.GetExamQuestionsAsync(_classId, _examId);
                if (!questionsResponse.Success || questionsResponse.Data == null)
                {
                    MessageBox.Show("Không thể tải danh sách câu hỏi.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }

                BindExamDetail(exam, questionsResponse.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void BindExamDetail(ExamResponseDto exam, List<ExamQuestionDetailDto> questions)
        {
            lblTitle.Text = exam.Title;
            
            string statusText = exam.IsPublished ? "Đã công bố" : "Bản nháp";
            lblInfo.Text = $"Lớp: {exam.ClassName} | " +
                          $"Thời lượng: {exam.Duration} phút | " +
                          $"Tổng điểm: {exam.TotalMarks} | " +
                          $"Số câu hỏi: {questions.Count} | " +
                          $"Trạng thái: {statusText} | " +
                          $"Bắt đầu: {exam.StartTime:dd/MM/yyyy HH:mm} | " +
                          $"Kết thúc: {exam.EndTime:dd/MM/yyyy HH:mm}";

            flowPanelQuestions.Controls.Clear();

            if (questions.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "Bài kiểm tra chưa có câu hỏi nào.",
                    AutoSize = true,
                    ForeColor = Color.FromArgb(80, 80, 80),
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    Padding = new Padding(20)
                };
                flowPanelQuestions.Controls.Add(lblEmpty);
                return;
            }

            // Sắp xếp theo QuestionOrder
            var sortedQuestions = questions.OrderBy(q => q.QuestionOrder).ToList();

            foreach (var question in sortedQuestions)
            {
                var questionPanel = CreateQuestionPanel(question, sortedQuestions.IndexOf(question) + 1);
                questionPanel.Width = flowPanelQuestions.ClientSize.Width - 40;
                questionPanel.Margin = new Padding(0, 0, 0, 20);
                flowPanelQuestions.Controls.Add(questionPanel);
            }
        }

        private Panel CreateQuestionPanel(ExamQuestionDetailDto question, int questionNumber)
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                AutoSize = false,                                // Quan trọng
                Width = flowPanelQuestions.ClientSize.Width - 60,
                Margin = new Padding(0, 0, 0, 20)
            };

            // Câu hỏi
            var lblQuestion = new Label
            {
                Text = $"Câu {questionNumber}: {question.QuestionText}",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                AutoSize = true,
                MaximumSize = new Size(panel.Width - 30, 0),     // Chống tràn chữ
                Location = new Point(10, 10)
            };
            panel.Controls.Add(lblQuestion);

            int yPos = lblQuestion.Bottom + 15;

            // Đáp án
            var sortedOptions = question.Options.OrderBy(o => o.OptionOrder).ToList();

            for (int i = 0; i < sortedOptions.Count; i++)
            {
                var option = sortedOptions[i];

                var lblOption = new Label
                {
                    Text = $"{(char)('A' + i)}. {option.OptionText}",
                    Font = new Font("Segoe UI", 11.5F, option.IsCorrect ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = option.IsCorrect ? Color.FromArgb(30, 160, 70) : Color.FromArgb(30, 30, 30),
                    AutoSize = true,
                    MaximumSize = new Size(panel.Width - 80, 0),
                    Location = new Point(30, yPos)
                };
                panel.Controls.Add(lblOption);

                // Tick ✓
                if (option.IsCorrect)
                {
                    var tick = new Label
                    {
                        Text = "✓",
                        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(30, 160, 70),
                        AutoSize = true,
                        Location = new Point(panel.Width - 40, yPos)
                    };
                    panel.Controls.Add(tick);
                }

                yPos = lblOption.Bottom + 8;
            }

            // Điểm số
            var lblMarks = new Label
            {
                Text = $"Điểm: {question.Marks}",
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                Location = new Point(30, yPos + 5)
            };
            panel.Controls.Add(lblMarks);

            // Set Height cuối
            panel.Height = lblMarks.Bottom + 20;

            return panel;
        }

    }
}

