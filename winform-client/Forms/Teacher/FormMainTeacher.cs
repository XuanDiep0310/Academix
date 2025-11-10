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
    public partial class FormMainTeacher : Form
    {

        private UserControl currentControl = null;

        public FormMainTeacher()
        {
            InitializeComponent();
        }

        private void ShowUserControl(UserControl uc)
        {
            if (currentControl != null)
            {
                this.Controls.Remove(currentControl);
                currentControl.Dispose();
            }

            uc.Dock = DockStyle.Fill;
            this.Controls.Add(uc);
            uc.BringToFront();

            currentControl = uc;
        }

        private void btnLopHocCuaToi_Click(object sender, EventArgs e)
        {
            ShowUserControl(new UC_MyClasses());
        }



        private void btnTaiLieu_Click(object sender, EventArgs e)
        {
            ShowUserControl(new UC_Materials());
        }

        private void btnNganHangCauHoi_Click(object sender, EventArgs e)
        {
            ShowUserControl(new UC_Questions());
        }

        private void btnBaiKiemTra_Click(object sender, EventArgs e)
        {
            ShowUserControl(new UC_Exams());
        }

        private void btnKetQua_Click(object sender, EventArgs e)
        {
            ShowUserControl(new UC_Result());
        }
    }
}
