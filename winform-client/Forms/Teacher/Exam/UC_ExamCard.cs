using Academix.WinApp.Forms.Teacher.Exam;
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
        public UC_ExamCard()
        {
            InitializeComponent();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Form_AddUpdateExam frm = new Form_AddUpdateExam();
            frm.Show();
        }
    }
}
