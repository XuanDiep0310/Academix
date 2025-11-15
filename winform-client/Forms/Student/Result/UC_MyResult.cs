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

            var card1 = new UC_ExamCard();
            flowpanelResult.Controls.Add(card1);
            var card2 = new UC_ExamCard();
            flowpanelResult.Controls.Add(card2);
        }
    }
}
