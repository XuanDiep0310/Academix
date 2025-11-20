using Academix.WinApp.Models.Student;
using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Exam
{
    public partial class UC_DoExamCard : UserControl
    {
        private ExamQuestionDto? _question;
        private int? _selectedOptionId;

        public event EventHandler<OptionSelectedEventArgs>? OptionSelected;

        public UC_DoExamCard()
        {
            InitializeComponent();
        }

        public void BindQuestion(ExamQuestionDto question, int? selectedOptionId = null)
        {
            _question = question;
            _selectedOptionId = selectedOptionId;

            lblCauHoi.Text = $"Câu {question.QuestionOrder}: {question.QuestionText}";

            flowOptions.SuspendLayout();
            flowOptions.Controls.Clear();
            flowOptions.Width = Width - 48;

            foreach (var option in question.Options.OrderBy(o => o.OptionOrder))
            {
                var radio = CreateOptionRadio(option);
                flowOptions.Controls.Add(radio);
            }

            flowOptions.ResumeLayout();
            UpdateControlHeight();
        }

        private Guna2RadioButton CreateOptionRadio(ExamOptionDto option)
        {
            var radio = new Guna2RadioButton
            {
                AutoSize = true,
                Text = option.OptionText,
                Tag = option.OptionId,
                Checked = _selectedOptionId == option.OptionId,
                Font = new Font("Segoe UI", 10.5f),
                Margin = new Padding(0, 6, 0, 6),
                Name = $"opt_{option.OptionId}"
            };

            radio.CheckedChanged += OptionRadio_CheckedChanged;
            return radio;
        }

        private void OptionRadio_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is not Guna2RadioButton radio || !radio.Checked || _question == null)
            {
                return;
            }

            if (radio.Tag is int optionId)
            {
                _selectedOptionId = optionId;
                OptionSelected?.Invoke(this, new OptionSelectedEventArgs(_question.QuestionId, optionId));
            }
        }

        private void UpdateControlHeight()
        {
            var totalOptionsHeight = flowOptions.Controls.Cast<Control>().Sum(c => c.Height + c.Margin.Vertical);
            Height = Math.Max(140, 80 + totalOptionsHeight);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            flowOptions.Width = Width - 48;
        }
    }

    public class OptionSelectedEventArgs : EventArgs
    {
        public OptionSelectedEventArgs(int questionId, int selectedOptionId)
        {
            QuestionId = questionId;
            SelectedOptionId = selectedOptionId;
        }

        public int QuestionId { get; }
        public int SelectedOptionId { get; }
    }
}
