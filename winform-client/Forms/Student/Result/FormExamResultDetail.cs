using Academix.WinApp.Models.Student;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Result
{
    public partial class FormExamResultDetail : Form
    {
        private readonly StudentExamResultDto _result;
        private Label lblTitle = null!;
        private Label lblScore = null!;
        private Label lblSummary = null!;
        private DataGridView dgvAnswers = null!;

        public FormExamResultDetail(StudentExamResultDto result)
        {
            _result = result;
            InitializeComponent();
            BindResult();
        }

        private void InitializeComponent()
        {
            Text = "Chi tiết bài kiểm tra";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            MinimumSize = new Size(800, 500);

            lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(20, 15)
            };

            lblScore = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                Location = new Point(20, 45)
            };

            lblSummary = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.DimGray,
                Location = new Point(20, 70)
            };

            dgvAnswers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Câu hỏi",
                FillWeight = 45,
                Name = "Question"
            });
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Đáp án đã chọn",
                FillWeight = 20,
                Name = "Selected"
            });
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Đáp án đúng",
                FillWeight = 20,
                Name = "Correct"
            });
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Đúng/Sai",
                FillWeight = 7,
                Name = "Status"
            });
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Điểm",
                FillWeight = 8,
                Name = "Marks"
            });

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110
            };
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblScore);
            topPanel.Controls.Add(lblSummary);

            Controls.Add(dgvAnswers);
            Controls.Add(topPanel);
        }

        private void BindResult()
        {
            lblTitle.Text = $"{_result.ExamTitle} (Attempt #{_result.AttemptId})";

            string percentageText = _result.Percentage.HasValue
                ? $"({_result.Percentage.Value:0.##}%)"
                : string.Empty;
            lblScore.Text =
                $"Điểm: {_result.TotalScore:0.##}/{_result.TotalMarks:0.##} {percentageText}";
            lblSummary.Text =
                $"Đúng {_result.CorrectAnswers}/{_result.TotalQuestions} câu | Trạng thái: {_result.Status}";

            dgvAnswers.Rows.Clear();

            if (_result.Answers == null || !_result.Answers.Any())
            {
                dgvAnswers.Rows.Add("Không có dữ liệu câu trả lời.", string.Empty, string.Empty, string.Empty, string.Empty);
                return;
            }

            foreach (var ans in _result.Answers)
            {
                dgvAnswers.Rows.Add(
                    ans.QuestionText,
                    ans.SelectedOptionText ?? "(Chưa chọn)",
                    ans.CorrectOptionText,
                    ans.IsCorrect ? "Đúng" : "Sai",
                    $"{ans.MarksObtained:0.##}/{ans.TotalMarks:0.##}"
                );
            }
        }
    }
}


