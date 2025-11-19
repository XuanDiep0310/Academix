using Academix.WinApp.Forms.Teacher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Exam
{
    public partial class UC_DoExam : UserControl
    {
        public UC_DoExam()
        {
            InitializeComponent();
        }
        TimeSpan remaining;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remaining.TotalSeconds <= 0)
            {
                timer1.Stop();
                lblClock.Text = "00:00:00";
                MessageBox.Show("Hết giờ!");
                return;
            }

            remaining = remaining.Subtract(TimeSpan.FromSeconds(1));
            lblClock.Text = remaining.ToString(@"hh\:mm\:ss");
        }

        private void LoadDoExam()
        {
            PanelDoExam.Controls.Clear();

            var card = new UC_DoExamCard();

            var card1 = new UC_DoExamCard();
            var card2 = new UC_DoExamCard();
            var card3 = new UC_DoExamCard();
            var card4 = new UC_DoExamCard();
            var card5 = new UC_DoExamCard();
            //var card = new UC_ClassCard(classId, name, code, studentCount);

            card.Margin = new Padding(10);
            PanelDoExam.Controls.Add(card);
            PanelDoExam.Controls.Add(card1);
            PanelDoExam.Controls.Add(card2);
            PanelDoExam.Controls.Add(card3);
            PanelDoExam.Controls.Add(card4);
            PanelDoExam.Controls.Add(card5);

        }

        private void UC_DoExam_Load(object sender, EventArgs e)
        {
            LoadDoExam();
        }
    }
}
