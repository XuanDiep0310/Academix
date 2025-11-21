using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Question;
using Academix.WinApp.Models.Teacher;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher
{
    public partial class UC_QuestionCard : UserControl
    {
        public event Func<Task>? OnUpdated;  // Dùng chung cho sửa & xóa

        [Browsable(false)]
        public int QuestionId { get; private set; }

        public UC_QuestionCard(QuestionResponseDto data)
        {
            InitializeComponent();
            LoadData(data);
        }

        private void LoadData(QuestionResponseDto data)
        {
            QuestionId = data.QuestionId;

            lblCauHoi.Text = data.QuestionText;
            lblMon.Text = data.Subject ?? "";
            lblDoKho.Text = ConvertDifficulty(data.DifficultyLevel);
            lblLoaiCauHoi.Text = ConvertQuestionType(data.QuestionType);

            var answers = data.Options
                              .OrderBy(o => o.OptionOrder)
                              .Select(o => o.OptionText)
                              .ToList();

            radDapAn1.Text = answers.ElementAtOrDefault(0) ?? "";
            radDapAn2.Text = answers.ElementAtOrDefault(1) ?? "";
            radDapAn3.Text = answers.ElementAtOrDefault(2) ?? "";
            radDapAn4.Text = answers.ElementAtOrDefault(3) ?? "";

            var correctIndex = data.Options
                                   .OrderBy(o => o.OptionOrder)
                                   .Select((o, idx) => new { o, idx })
                                   .FirstOrDefault(x => x.o.IsCorrect)?.idx ?? -1;

            radDapAn1.Checked = correctIndex == 0;
            radDapAn2.Checked = correctIndex == 1;
            radDapAn3.Checked = correctIndex == 2;
            radDapAn4.Checked = correctIndex == 3;

            SetCorrectColor(radDapAn1, correctIndex == 0);
            SetCorrectColor(radDapAn2, correctIndex == 1);
            SetCorrectColor(radDapAn3, correctIndex == 2);
            SetCorrectColor(radDapAn4, correctIndex == 3);
        }

        private void SetCorrectColor(RadioButton rdo, bool isCorrect)
        {
            rdo.BackColor = isCorrect ? Color.Aquamarine : Color.Transparent;
        }

        private async void btnSua_Click(object sender, EventArgs e)
        {
            if (QuestionId <= 0) return;

            using var frm = new Form_AddUpdateQuestion(QuestionId);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (OnUpdated != null)
                    await OnUpdated.Invoke();
            }
        }

        private async void btnXoa_Click(object sender, EventArgs e)
        {
            if (QuestionId <= 0) return;

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                var api = new QuestionApiService();
                var result = await api.DeleteQuestionAsync(QuestionId);

                if (result.Success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (OnUpdated != null)
                        await OnUpdated.Invoke();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại: " + result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xảy ra lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConvertDifficulty(string level) => level switch
        {
            "Easy" => "Dễ",
            "Medium" => "Trung bình",
            "Hard" => "Khó",
            _ => "Không rõ"
        };

        private string ConvertQuestionType(string type) => type switch
        {
            "SingleChoice" => "Một đáp án",
            "MultiChoice" => "Nhiều đáp án",
            _ => "Không rõ"
        };
    }
}
