using Academix.WinApp.Forms.Student.Exam;
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
    public partial class UC_ExamCard : UserControl
    {
        public UC_ExamCard()
        {
            InitializeComponent();
        }

        private void btnBatDauLamBai_Click(object sender, EventArgs e)
        {
            FormMainStudent frm = Application.OpenForms["FormMainStudent"] as FormMainStudent;

            if (frm != null)
            {
                frm.mainPanel.Controls.Clear();
                UC_DoExam uc = new UC_DoExam();
                uc.Dock = DockStyle.Fill;
                frm.mainPanel.Controls.Add(uc);
            }
            else
            {
                MessageBox.Show("FormMainStudent chưa mở!");
            }
        }
    }
}
