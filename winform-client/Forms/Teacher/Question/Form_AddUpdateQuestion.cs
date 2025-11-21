using Academix.WinApp.Api;
using Academix.WinApp.Models.Common;
using Academix.WinApp.Models.Teacher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher.Question
{
    public partial class Form_AddUpdateQuestion : Form
    {
        private readonly int _questionId; // 0 => add, >0 => update
        private readonly QuestionApiService _api = new QuestionApiService();
        private int _correctAnswerIndex = -1; // lưu đáp án đúng: 0-3

        public Form_AddUpdateQuestion(int questionId = 0)
        {
            InitializeComponent();
            _questionId = questionId;
            _api = new QuestionApiService();

        }

        private async void Form_AddUpdateQuestion_Load(object sender, EventArgs e)
        {
            cmbDoKho.DisplayMember = "Text";
            cmbDoKho.ValueMember = "Value";

            var list = new List<DifficultyItem>
                {
                    new DifficultyItem { Text = "Dễ", Value = "Easy" },
                    new DifficultyItem { Text = "Trung bình", Value = "Medium" },
                    new DifficultyItem { Text = "Khó", Value = "Hard" }
                };

            cmbDoKho.DataSource = list;

            if (_questionId > 0)
            {
                Text = "Cập nhật câu hỏi";
                btnThem.Text = "Cập nhật";
                await LoadQuestion();
            }
            else
            {
                Text = "Thêm câu hỏi mới";
            }
        }

        private async Task LoadQuestion()
        {
            try
            {
                var response = await _api.GetQuestionByIdAsync(_questionId);
                if (response.Success && response.Data != null)
                {
                    var q = response.Data;

                    txtCauHoi.Text = q.QuestionText;
                    txtMonHoc.Text = q.Subject ?? "";
                    cmbDoKho.SelectedValue = q.DifficultyLevel ?? "";

                    // Lấy danh sách đáp án theo OptionOrder
                    var answers = q.Options
                                   .OrderBy(o => o.OptionOrder)
                                   .Select(o => o.OptionText)
                                   .ToList();

                    txtDapAn1.Text = answers.ElementAtOrDefault(0) ?? "";
                    txtDapAn2.Text = answers.ElementAtOrDefault(1) ?? "";
                    txtDapAn3.Text = answers.ElementAtOrDefault(2) ?? "";
                    txtDapAn4.Text = answers.ElementAtOrDefault(3) ?? "";

                    // Tìm đáp án đúng
                    _correctAnswerIndex = q.Options
                                             .OrderBy(o => o.OptionOrder)
                                             .Select((o, idx) => new { o, idx })
                                             .FirstOrDefault(x => x.o.IsCorrect)?.idx ?? -1;

                    UpdateCorrectAnswerButtonUI();
                }
                else
                {
                    MessageBox.Show("Không tải được câu hỏi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnChon1_Click(object sender, EventArgs e) => SetCorrectAnswer(0);
        private void btnChon2_Click(object sender, EventArgs e) => SetCorrectAnswer(1);
        private void btnChon3_Click(object sender, EventArgs e) => SetCorrectAnswer(2);
        private void btnChon4_Click(object sender, EventArgs e) => SetCorrectAnswer(3);

        private void SetCorrectAnswer(int index)
        {
            _correctAnswerIndex = index;
            UpdateCorrectAnswerButtonUI();
        }

        private void UpdateCorrectAnswerButtonUI()
        {
            btnChon1.FillColor = (_correctAnswerIndex == 0) ? Color.Lime : Color.Silver;
            btnChon2.FillColor = (_correctAnswerIndex == 1) ? Color.Lime : Color.Silver;
            btnChon3.FillColor = (_correctAnswerIndex == 2) ? Color.Lime : Color.Silver;
            btnChon4.FillColor = (_correctAnswerIndex == 3) ? Color.Lime : Color.Silver;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtCauHoi.Text)) return false;
            if (string.IsNullOrWhiteSpace(txtMonHoc.Text)) return false;

            if (cmbDoKho.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn độ khó!", "Thiếu dữ liệu",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_correctAnswerIndex < 0) return false;

            return true;
        }




        private async void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            var options = new List<CreateQuestionOptionDto>();

            options.Add(new CreateQuestionOptionDto { OptionText = txtDapAn1.Text.Trim(), IsCorrect = (_correctAnswerIndex == 0), OptionOrder = 1 });
            options.Add(new CreateQuestionOptionDto { OptionText = txtDapAn2.Text.Trim(), IsCorrect = (_correctAnswerIndex == 1), OptionOrder = 2 });
            options.Add(new CreateQuestionOptionDto { OptionText = txtDapAn3.Text.Trim(), IsCorrect = (_correctAnswerIndex == 2), OptionOrder = 3 });
            options.Add(new CreateQuestionOptionDto { OptionText = txtDapAn4.Text.Trim(), IsCorrect = (_correctAnswerIndex == 3), OptionOrder = 4 });

            // Loại option rỗng
            options = options.Where(o => !string.IsNullOrWhiteSpace(o.OptionText)).ToList();

            if (options.Count < 2)
            {
                MessageBox.Show("Cần ít nhất 2 đáp án!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!options.Any(o => o.IsCorrect))
            {
                MessageBox.Show("Bạn chưa chọn đáp án đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string difficulty = cmbDoKho.SelectedValue.ToString();   // LUÔN AN TOÀN VÌ ValidateForm ĐÃ CHECK

            if (_questionId == 0)
            {
                // THÊM MỚI
                var request = new CreateQuestionRequestDto
                {
                    QuestionText = txtCauHoi.Text.Trim(),
                    Subject = txtMonHoc.Text.Trim(),
                    DifficultyLevel = difficulty,
                    QuestionType = "SingleChoice",
                    Options = options
                };

                var result = await _api.CreateQuestionAsync(request);
                if (result.Success)
                {
                    MessageBox.Show("Thêm thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // CẬP NHẬT
                var request = new UpdateQuestionRequestDto
                {
                    QuestionText = txtCauHoi.Text.Trim(),
                    Subject = txtMonHoc.Text.Trim(),
                    DifficultyLevel = difficulty,
                    Options = options
                };

                var result = await _api.UpdateQuestionAsync(_questionId, request);
                if (result.Success)
                {
                    MessageBox.Show("Cập nhật thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đóng không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        class DifficultyItem
{
    public string Text { get; set; }
    public string Value { get; set; }
}


    }

}
