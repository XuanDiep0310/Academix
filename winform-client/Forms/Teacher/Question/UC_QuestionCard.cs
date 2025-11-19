using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Question;
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

namespace Academix.WinApp.Forms.Teacher
{

    public partial class UC_QuestionCard : UserControl
    {
        public event Func<Task>? OnUpdated; 

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int _questionId { get; set; }


        public UC_QuestionCard(QuestionResponseDto data)
        {
            InitializeComponent();
            LoadData(data);
        }
        private void LoadData(QuestionResponseDto data)
        {
            _questionId = data.QuestionId;

            lblCauHoi.Text = data.QuestionText;
            lblMon.Text = data.Subject ?? "";
            lblDoKho.Text = data.DifficultyLevel ?? "";
            lblLoaiCauHoi.Text = data.QuestionType ?? "";

            // Lấy danh sách đáp án theo thứ tự
            var answers = data.Options
                              .OrderBy(o => o.OptionOrder)
                              .Select(o => o.OptionText)
                              .ToList();

            // Gán đáp án vào các radio button
            radDapAn1.Text = answers.ElementAtOrDefault(0) ?? "";
            radDapAn2.Text = answers.ElementAtOrDefault(1) ?? "";
            radDapAn3.Text = answers.ElementAtOrDefault(2) ?? "";
            radDapAn4.Text = answers.ElementAtOrDefault(3) ?? "";

            // Tìm đáp án đúng
            var correctIndex = data.Options
                                   .OrderBy(o => o.OptionOrder)
                                   .Select((o, idx) => new { o, idx })
                                   .FirstOrDefault(x => x.o.IsCorrect)?.idx ?? -1;

            // Check đáp án đúng
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
            if (isCorrect)
                rdo.BackColor = Color.Aquamarine;
        }


        public event EventHandler? OnDataChanged;

        private async void btnSua_Click(object sender, EventArgs e)
        {
            if (_questionId <= 0)
                return;

            using var frm = new Form_AddUpdateQuestion(_questionId);
            var dialogResult = frm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                if (OnUpdated != null)
                    await OnUpdated.Invoke(); // Gọi callback
            }
        }
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            if (_questionId <= 0)
                return;

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                var api = new QuestionApiService();
                var result = await api.DeleteQuestionAsync(_questionId);

                if (result.Success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OnDataChanged?.Invoke(this, EventArgs.Empty);
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

    }
}
