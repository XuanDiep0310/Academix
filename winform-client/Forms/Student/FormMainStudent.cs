using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms.Student.MyResult;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student
{
    public partial class FormMainStudent : Form
    {
        public FormMainStudent()
        {
            InitializeComponent();
        }

        private void btnMyClass_Click(object sender, EventArgs e)
        {
            //ResetTabButtons();

            //btnQLTaiKhoan.FillColor = Color.White;
            //btnQLTaiKhoan.ForeColor = Color.LightSkyBlue;

            mainPanel.Controls.Clear();

            UC_LopHocCuaToi uc = new UC_LopHocCuaToi();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnTaiLieuHocTap_Click(object sender, EventArgs e)
        {
            mainPanel.Controls.Clear();

            UC_MyMaterials uc = new UC_MyMaterials();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnBaiKiemTra_Click(object sender, EventArgs e)
        {
            mainPanel.Controls.Clear();

            UC_MyExams uc = new UC_MyExams();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnMyResult_Click(object sender, EventArgs e)
        {
            mainPanel.Controls.Clear();
            UC_MyResult uc = new UC_MyResult();
            uc.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(uc);

        }
    }
}
