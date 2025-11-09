using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormMainAdmin : Form
    {
        public FormMainAdmin()
        {
            InitializeComponent();
            ResetTabButtons();
        }

        private void ResetTabButtons()
        {
            Color defaultColor = Color.LightSkyBlue; // Xanh dương mặc định
            Color defaultTextColor = Color.White;

            btnQLTaiKhoan.FillColor = defaultColor;
            btnQLTaiKhoan.ForeColor = defaultTextColor;

            btnQLLopHoc.FillColor = defaultColor;
            btnQLLopHoc.ForeColor = defaultTextColor;

            btnCaiDat.FillColor = defaultColor;
            btnCaiDat.ForeColor = defaultTextColor;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnQLTaiKhoan.FillColor = Color.White; // Tab được chọn -> trắng
            btnQLTaiKhoan.ForeColor = Color.LightSkyBlue; // Chữ xanh



        }
    }
}
