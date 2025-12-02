using Academix.WinApp.Api;
using Academix.WinApp.Models.Student;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher.Result
{
    public partial class Form_StudentExamResultDetail : Form
    {
        private readonly int _attemptId;
        private readonly string _studentName;
        private readonly ExamApiService _examApi = new ExamApiService();
        
        private Guna.UI2.WinForms.Guna2Panel topPanel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStudentName;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblScore;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSummary;
        private Guna.UI2.WinForms.Guna2DataGridView dgvAnswers;
        private Panel mainPanel;

        public Form_StudentExamResultDetail(int attemptId, string studentName)
        {
            _attemptId = attemptId;
            _studentName = studentName;
            InitializeComponent();
            LoadResultAsync();
        }

        private void InitializeComponent()
        {
            topPanel = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblStudentName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblScore = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblSummary = new Guna.UI2.WinForms.Guna2HtmlLabel();
            dgvAnswers = new Guna.UI2.WinForms.Guna2DataGridView();
            mainPanel = new Panel();
            
            topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAnswers).BeginInit();
            mainPanel.SuspendLayout();
            SuspendLayout();
            
            // topPanel
            topPanel.BackColor = Color.FromArgb(240, 248, 255);
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblStudentName);
            topPanel.Controls.Add(lblScore);
            topPanel.Controls.Add(lblSummary);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(20);
            topPanel.Size = new Size(1000, 150);
            topPanel.TabIndex = 0;
            
            // lblTitle
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(70, 130, 180);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(960, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Đang tải...";
            
            // lblStudentName
            lblStudentName.BackColor = Color.Transparent;
            lblStudentName.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            lblStudentName.ForeColor = Color.FromArgb(50, 50, 50);
            lblStudentName.Location = new Point(20, 55);
            lblStudentName.Name = "lblStudentName";
            lblStudentName.Size = new Size(960, 25);
            lblStudentName.TabIndex = 1;
            lblStudentName.Text = $"Học sinh: {_studentName}";
            
            // lblScore
            lblScore.BackColor = Color.Transparent;
            lblScore.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblScore.ForeColor = Color.FromArgb(70, 130, 180);
            lblScore.Location = new Point(20, 85);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(400, 25);
            lblScore.TabIndex = 2;
            lblScore.Text = "Điểm: --/--";
            
            // lblSummary
            lblSummary.BackColor = Color.Transparent;
            lblSummary.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblSummary.ForeColor = Color.FromArgb(100, 100, 100);
            lblSummary.Location = new Point(20, 115);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new Size(960, 25);
            lblSummary.TabIndex = 3;
            lblSummary.Text = "Đang tải...";
            
            // dgvAnswers
            dgvAnswers.AllowUserToAddRows = false;
            dgvAnswers.AllowUserToDeleteRows = false;
            dgvAnswers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAnswers.BackgroundColor = Color.White;
            dgvAnswers.ColumnHeadersHeight = 40;
            dgvAnswers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvAnswers.Dock = DockStyle.Fill;
            dgvAnswers.GridColor = Color.FromArgb(231, 229, 255);
            dgvAnswers.Location = new Point(0, 0);
            dgvAnswers.Name = "dgvAnswers";
            dgvAnswers.ReadOnly = true;
            dgvAnswers.RowHeadersVisible = false;
            dgvAnswers.RowHeadersWidth = 51;
            dgvAnswers.RowTemplate.Height = 40;
            dgvAnswers.Size = new Size(1000, 450);
            dgvAnswers.TabIndex = 1;
            dgvAnswers.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.FeterRiver;
            dgvAnswers.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvAnswers.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvAnswers.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvAnswers.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvAnswers.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvAnswers.ThemeStyle.BackColor = Color.White;
            dgvAnswers.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvAnswers.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(70, 130, 180);
            dgvAnswers.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvAnswers.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvAnswers.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvAnswers.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvAnswers.ThemeStyle.HeaderStyle.Height = 40;
            dgvAnswers.ThemeStyle.ReadOnly = true;
            dgvAnswers.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvAnswers.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAnswers.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvAnswers.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvAnswers.ThemeStyle.RowsStyle.Height = 40;
            dgvAnswers.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvAnswers.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            
            SetupDataGridView();
            
            // mainPanel
            mainPanel.Controls.Add(dgvAnswers);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 150);
            mainPanel.Name = "mainPanel";
            mainPanel.Padding = new Padding(20);
            mainPanel.Size = new Size(1000, 450);
            mainPanel.TabIndex = 1;
            
            // Form
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(mainPanel);
            Controls.Add(topPanel);
            MinimumSize = new Size(800, 500);
            Name = "Form_StudentExamResultDetail";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chi tiết bài kiểm tra của học sinh";
            
            topPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAnswers).EndInit();
            mainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void SetupDataGridView()
        {
            dgvAnswers.Columns.Clear();
            
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QuestionText",
                HeaderText = "Câu hỏi",
                DataPropertyName = "QuestionText",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    WrapMode = DataGridViewTriState.True,
                    Padding = new Padding(10, 5, 10, 5)
                }
            });
            
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SelectedOption",
                HeaderText = "Đáp án đã chọn",
                DataPropertyName = "SelectedOptionText",
                ReadOnly = true,
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    ForeColor = Color.FromArgb(220, 53, 69),
                    WrapMode = DataGridViewTriState.True
                }
            });
            
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CorrectOption",
                HeaderText = "Đáp án đúng",
                DataPropertyName = "CorrectOptionText",
                ReadOnly = true,
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    ForeColor = Color.FromArgb(40, 167, 69),
                    WrapMode = DataGridViewTriState.True
                }
            });
            
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Trạng thái",
                DataPropertyName = "Status",
                ReadOnly = true,
                Width = 100
            });
            
            dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Marks",
                HeaderText = "Điểm",
                DataPropertyName = "Marks",
                ReadOnly = true,
                Width = 100
            });
        }

        private async void LoadResultAsync()
        {
            try
            {
                var result = await _examApi.GetAttemptResultAsync(_attemptId);
                
                if (result == null)
                {
                    MessageBox.Show("Không thể tải chi tiết kết quả bài kiểm tra.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                
                BindResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải kết quả: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void BindResult(StudentExamResultDto result)
        {
            lblTitle.Text = $"{result.ExamTitle} (Attempt #{result.AttemptId})";
            
            string percentageText = result.Percentage.HasValue
                ? $"({result.Percentage.Value:0.##}%)"
                : string.Empty;
            lblScore.Text = $"Điểm: {result.TotalScore:0.##}/{result.TotalMarks:0.##} {percentageText}";
            
            lblSummary.Text = $"Đúng {result.CorrectAnswers}/{result.TotalQuestions} câu | " +
                             $"Trạng thái: {result.Status} | " +
                             $"Bắt đầu: {result.StartTime:dd/MM/yyyy HH:mm} | " +
                             $"Nộp bài: {(result.SubmitTime.HasValue ? result.SubmitTime.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa nộp")}";

            dgvAnswers.Rows.Clear();

            if (result.Answers == null || !result.Answers.Any())
            {
                dgvAnswers.Rows.Add("Không có dữ liệu câu trả lời.", string.Empty, string.Empty, string.Empty, string.Empty);
                return;
            }

            foreach (var ans in result.Answers)
            {
                // Convert isCorrect (object) to bool? an toàn
                bool? isCorrectVal = null;
                if (ans.IsCorrect is bool b) isCorrectVal = b;
                else if (ans.IsCorrect is int i) isCorrectVal = i != 0;
                else if (ans.IsCorrect is string s)
                {
                    if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1") 
                        isCorrectVal = true;
                    else if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || s == "0") 
                        isCorrectVal = false;
                }
                
                string statusText = isCorrectVal == true ? "✓ Đúng" : (isCorrectVal == false ? "✗ Sai" : "?");
                Color statusColor = isCorrectVal == true ? Color.FromArgb(40, 167, 69) : 
                                   (isCorrectVal == false ? Color.FromArgb(220, 53, 69) : Color.Gray);
                
                int rowIndex = dgvAnswers.Rows.Add(
                    ans.QuestionText,
                    ans.SelectedOptionText ?? "(Chưa chọn)",
                    ans.CorrectOptionText,
                    statusText,
                    $"{ans.MarksObtained:0.##}/{ans.TotalMarks:0.##}"
                );
                
                // Tô màu hàng dựa trên kết quả
                if (isCorrectVal == true)
                {
                    dgvAnswers.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218);
                }
                else if (isCorrectVal == false)
                {
                    dgvAnswers.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(248, 215, 218);
                }
                
                dgvAnswers.Rows[rowIndex].Cells["Status"].Style.ForeColor = statusColor;
                dgvAnswers.Rows[rowIndex].Cells["Status"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
        }
    }
}

