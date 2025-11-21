using Academix.WinApp.Api;
using Academix.WinApp.Models.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Result
{
    public partial class UCResultCard : UserControl
    {
        private StudentExamResultDto? _result;

        public UCResultCard()
        {
            InitializeComponent();
        }

        public void Bind(StudentExamResultDto result)
        {
            _result = result;

            guna2HtmlLabel1.Text = result.ExamTitle;
            guna2HtmlLabel2.Text = !string.IsNullOrEmpty(result.ClassName) ? result.ClassName : "Chưa có thông tin";
            guna2HtmlLabel3.Text = result.StartTime.ToString("yyyy-MM-dd HH:mm");

            guna2HtmlLabel5.Text = result.TotalScore.ToString("0.##");
            guna2HtmlLabel4.Text = $"/{result.TotalMarks:0.##}";
            guna2HtmlLabel6.Text = $"Đúng {result.CorrectAnswers}/{result.TotalQuestions} câu - {result.Status}";

            guna2Button1.Click -= guna2Button1_Click;
            guna2Button1.Click += guna2Button1_Click;
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            if (_result == null)
                return;

            try
            {
                StudentExamResultDto detail = _result;

                if (detail.Answers == null || detail.Answers.Count == 0)
                {
                    var api = new ExamApiService();
                    var loaded = await api.GetAttemptResultAsync(_result.AttemptId);
                    if (loaded != null)
                    {
                        detail = loaded;
                    }
                }

                using var dlg = new FormExamResultDetail(detail);
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải chi tiết kết quả.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
