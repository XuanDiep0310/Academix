using Academix.WinApp.Models.Student;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Result
{
    public partial class FormExamResultDetail : Form
    {
        private readonly StudentExamResultDto _result;

        private Label lblTitle;
        private Label lblScore;
        private Label lblSummary;
        private Panel topPanel;
        private DataGridView dgvAnswers;

        public FormExamResultDetail(StudentExamResultDto result)
        {
            _result = result;
            InitializeComponent();
            BindResult();
        }

        private void InitializeComponent()
        {
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.White;

            // ==== TOP PANEL (Header) ====
            topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 130;
            topPanel.Padding = new Padding(20, 15, 20, 15);
            topPanel.BackColor = Color.FromArgb(245, 247, 250);

            lblTitle = new Label();
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Top = 10;

            lblScore = new Label();
            lblScore.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            lblScore.ForeColor = Color.FromArgb(33, 150, 243);
            lblScore.Top = 55;
            lblScore.AutoSize = true;

            lblSummary = new Label();
            lblSummary.Font = new Font("Segoe UI", 11F, FontStyle.Italic);
            lblSummary.ForeColor = Color.FromArgb(90, 90, 90);
            lblSummary.Top = 90;
            lblSummary.AutoSize = true;

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblScore);
            topPanel.Controls.Add(lblSummary);

            // ==== DATAGRIDVIEW ====
            dgvAnswers = new DataGridView();
            dgvAnswers.Dock = DockStyle.Fill;
            dgvAnswers.ReadOnly = true;
            dgvAnswers.RowHeadersVisible = false;
            dgvAnswers.AllowUserToAddRows = false;
            dgvAnswers.BackgroundColor = Color.White;
            dgvAnswers.BorderStyle = BorderStyle.None;

            dgvAnswers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAnswers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvAnswers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
            dgvAnswers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvAnswers.EnableHeadersVisualStyles = false;

            dgvAnswers.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvAnswers.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvAnswers.RowTemplate.Height = 60;

            dgvAnswers.Columns.Add("QuestionText", "Câu hỏi");
            dgvAnswers.Columns.Add("Selected", "Đáp án chọn");
            dgvAnswers.Columns.Add("Correct", "Đáp án đúng");
            //dgvAnswers.Columns.Add("Status", "Kết quả");
            dgvAnswers.Columns.Add("Marks", "Điểm");

            // ==== FORM ====
            ClientSize = new Size(900, 600);
            MinimumSize = new Size(850, 550);
            Text = "Chi tiết bài kiểm tra";
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(dgvAnswers);
            Controls.Add(topPanel);
        }

        private void BindResult()
        {
            // ==== HEADER INFO ====
            lblTitle.Text = $"{_result.ExamTitle} (Lần làm: #{_result.AttemptId})";

            string percentageText = _result.Percentage.HasValue
                ? $"({_result.Percentage.Value:0.##}%)"
                : "";

            lblScore.Text =
                $"Điểm: {_result.TotalScore:0.##}/{_result.TotalMarks:0.##} {percentageText}";

            lblSummary.Text =
                $"Đúng {_result.CorrectAnswers}/{_result.TotalQuestions} | Trạng thái: {_result.Status}";

            // ==== TABLE DATA ====
            dgvAnswers.Rows.Clear();

            if (_result.Answers == null || !_result.Answers.Any())
            {
                dgvAnswers.Rows.Add("Không có dữ liệu câu trả lời.", "", "", "", "");
                return;
            }

            foreach (var ans in _result.Answers)
            {
                bool? isCorrectVal = null;

                if (ans.IsCorrect is bool b) isCorrectVal = b;
                else if (ans.IsCorrect is int i) isCorrectVal = i != 0;
                else if (ans.IsCorrect is string s)
                {
                    if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1") isCorrectVal = true;
                    else if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || s == "0") isCorrectVal = false;
                }

                string statusText = isCorrectVal == true ? "Đúng" :
                                    isCorrectVal == false ? "Sai" : "?";

                dgvAnswers.Rows.Add(
                    ans.QuestionText,
                    ans.SelectedOptionText ?? "(Chưa chọn)",
                    ans.CorrectOptionText,
                    $"{ans.MarksObtained:0.##}/{ans.TotalMarks:0.##}"
                );
            }

            //// ==== ROW COLORING (Đúng = xanh, Sai = đỏ) ====
            //foreach (DataGridViewRow row in dgvAnswers.Rows)
            //{
            //    string status = row.Cells[3].Value?.ToString();

            //    if (status == "Đúng")
            //        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 230); // Xanh nhạt

            //    else if (status == "Sai")
            //        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230); // Đỏ nhạt
            //}
        }
    }
}
