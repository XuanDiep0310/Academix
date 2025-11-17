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
    public partial class UC_MyExams : UserControl
    {
        public UC_MyExams()
        {
            InitializeComponent();
        }

        private async void UC_MyExams_Load(object sender, EventArgs e)
        {
            await LoadExams();
        }

        private async Task LoadExams()
        {

            var card1 = new UC_ExamCard();
            flowpanelExams.Controls.Add(card1);
            var card2 = new UC_ExamCard();
            flowpanelExams.Controls.Add(card2);
        }
    }
}
