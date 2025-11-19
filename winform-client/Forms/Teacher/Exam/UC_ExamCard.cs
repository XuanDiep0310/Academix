using Academix.WinApp.Forms.Teacher.Exam;
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
    public partial class UC_ExamCard : UserControl
    {
        public event Func<Task>? OnUpdated; // callback reload

        private ExamResponseDto _exam;

        public UC_ExamCard(ExamResponseDto exam)
        {
            InitializeComponent();
            _exam = exam;
        }

        private async void btnSua_Click(object sender, EventArgs e)
        {
            Form_AddUpdateExam frm = new Form_AddUpdateExam(_exam.ClassId, _exam.ExamId);

            // Gọi callback khi form lưu
            frm.OnSaved += async () =>
            {
                if (OnUpdated != null)
                    await OnUpdated();
            };

            frm.Show();
        }
    }

}
