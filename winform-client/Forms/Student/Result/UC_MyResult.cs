using Academix.WinApp.Api;
using Academix.WinApp.Forms.Student.Result;
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
                var history = await examApi.GetStudentExamHistoryAsync();

                var ordered = history
                    .OrderByDescending(r => r.StartTime)
                    .ToList();

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
    }
}
