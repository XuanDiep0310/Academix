using Academix.WinApp.Forms.Teacher.Exam;
using Academix.WinApp.Forms.Teacher.Question;
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
    public partial class UC_Exams : UserControl
    {
        public UC_Exams()
        {
            InitializeComponent();
        }

        private void LoadExams()
        {
            flowPanelExams.Controls.Clear();

            var card = new UC_ExamCard();
            card.Dock = DockStyle.Fill;

            var card1 = new UC_ExamCard();
            var card2 = new UC_ExamCard();
            var card3 = new UC_ExamCard();
            var card4 = new UC_ExamCard();
            var card5 = new UC_ExamCard();
            //var card = new UC_ClassCard(classId, name, code, studentCount);

            card.Margin = new Padding(10);
            flowPanelExams.Controls.Add(card);
            flowPanelExams.Controls.Add(card1);
            flowPanelExams.Controls.Add(card2);
            flowPanelExams.Controls.Add(card3);
            flowPanelExams.Controls.Add(card4);
            flowPanelExams.Controls.Add(card5);

        }

        private void UC_Exams_Load(object sender, EventArgs e)
        {
            LoadExams();
        }

        private void btnTaoBaiKiemTra_Click(object sender, EventArgs e)
        {
            Form_AddUpdateExam frm = new Form_AddUpdateExam();
            frm.Show();
        }
    }
}
