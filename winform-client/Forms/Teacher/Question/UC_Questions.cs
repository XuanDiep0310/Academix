using Academix.WinApp.Forms.Teacher.Material;
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
    public partial class UC_Questions : UserControl
    {
        public UC_Questions()
        {
            InitializeComponent();
        }

        private void LoadQuestions()
        {
            flowPanelQuestion.Controls.Clear();

            var card = new UC_QuestionCard();
            card.Dock = DockStyle.Fill;

            var card1 = new UC_QuestionCard();
            var card2 = new UC_QuestionCard();
            var card3 = new UC_QuestionCard();
            var card4 = new UC_QuestionCard();
            var card5 = new UC_QuestionCard();
            //var card = new UC_ClassCard(classId, name, code, studentCount);

            card.Margin = new Padding(10);
            flowPanelQuestion.Controls.Add(card);
            flowPanelQuestion.Controls.Add(card1);
            flowPanelQuestion.Controls.Add(card2);
            flowPanelQuestion.Controls.Add(card3);
            flowPanelQuestion.Controls.Add(card4);
            flowPanelQuestion.Controls.Add(card5);

        }

        private void UC_Questions_Load(object sender, EventArgs e)
        {
            LoadQuestions();
        }

        private void btnThemCauHoi_Click(object sender, EventArgs e)
        {
            Form_AddUpdateQuestion frm = new Form_AddUpdateQuestion();
            frm.ShowDialog();
        }
    }
}
