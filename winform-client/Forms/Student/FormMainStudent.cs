using Academix.WinApp.Forms.Admin;
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

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

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
    }
}
