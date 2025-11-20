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
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private Panel topPanel;
        private DataGridView dgvAnswers = null!;

        public FormExamResultDetail(StudentExamResultDto result)
        {
            _result = result;
            InitializeComponent();
            BindResult();
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblScore = new Label();
            lblSummary = new Label();
            dgvAnswers = new DataGridView();
            topPanel = new Panel();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvAnswers).BeginInit();
            topPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(100, 23);
            lblTitle.TabIndex = 0;
            // 
            // lblScore
            // 
            lblScore.Location = new Point(0, 0);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(100, 23);
            lblScore.TabIndex = 1;
            // 
            // lblSummary
            // 
            lblSummary.Location = new Point(0, 0);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new Size(100, 23);
            lblSummary.TabIndex = 2;
            // 
            // dgvAnswers
            // 
            dgvAnswers.ColumnHeadersHeight = 29;
            dgvAnswers.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
            dgvAnswers.Location = new Point(0, 0);
            dgvAnswers.Name = "dgvAnswers";
            dgvAnswers.RowHeadersWidth = 51;
            dgvAnswers.Size = new Size(773, 150);
            dgvAnswers.TabIndex = 0;
            // 
            // topPanel
            // 
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblScore);
            topPanel.Controls.Add(lblSummary);
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(200, 100);
            topPanel.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.MinimumWidth = 6;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 125;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.MinimumWidth = 6;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 125;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.MinimumWidth = 6;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.Width = 125;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.MinimumWidth = 6;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.Width = 125;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.MinimumWidth = 6;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.Width = 125;
            // 
            // FormExamResultDetail
            // 
            ClientSize = new Size(882, 553);
            Controls.Add(dgvAnswers);
            Controls.Add(topPanel);
            MinimumSize = new Size(800, 500);
            Name = "FormExamResultDetail";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chi tiết bài kiểm tra";
            ((System.ComponentModel.ISupportInitialize)dgvAnswers).EndInit();
            topPanel.ResumeLayout(false);
            ResumeLayout(false);
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
                // Convert isCorrect (object) to bool? an toàn
                bool? isCorrectVal = null;
                if (ans.IsCorrect is bool b) isCorrectVal = b;
                else if (ans.IsCorrect is int i) isCorrectVal = i != 0;
                else if (ans.IsCorrect is string s) {
                    if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1") isCorrectVal = true;
                    else if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || s == "0") isCorrectVal = false;
                }
                string statusText = isCorrectVal == true ? "Đúng" : (isCorrectVal == false ? "Sai" : "?");
                dgvAnswers.Rows.Add(
                    ans.QuestionText,
                    ans.SelectedOptionText ?? "(Chưa chọn)",
                    ans.CorrectOptionText,
                    statusText,
                    $"{ans.MarksObtained:0.##}/{ans.TotalMarks:0.##}"
                );
            }
        }
    }
}


