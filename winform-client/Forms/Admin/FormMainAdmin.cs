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

            btnDoiMatKhau.FillColor = defaultColor;
            btnDoiMatKhau.ForeColor = defaultTextColor;

            btnDangXuat.FillColor = defaultColor;
            btnDangXuat.ForeColor = defaultTextColor;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnQLTaiKhoan.FillColor = Color.White;
            btnQLTaiKhoan.ForeColor = Color.LightSkyBlue;

            mainPanel.Controls.Clear();

            UC_QLTaiKhoan uc = new UC_QLTaiKhoan();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnQLLopHoc_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnQLLopHoc.FillColor = Color.White; // Tab được chọn -> trắng
            btnQLLopHoc.ForeColor = Color.LightSkyBlue; // Chữ xanh

            mainPanel.Controls.Clear();
            UC_QLLopHoc uc = new UC_QLLopHoc();
            uc.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(uc);

        }

        private void btnCaiDat_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnCaiDat.FillColor = Color.White; // Tab được chọn -> trắng
            btnCaiDat.ForeColor = Color.LightSkyBlue; // Chữ xanh

            mainPanel.Controls.Clear();
            UC_CaiDat uc = new UC_CaiDat();
            uc.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(uc);
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnDoiMatKhau.FillColor = Color.White; // Tab được chọn -> trắng
            btnDoiMatKhau.ForeColor = Color.LightSkyBlue; // Chữ xanh
            FormDoiMatKhau form = new FormDoiMatKhau();
            form.ShowDialog();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnDangXuat.FillColor = Color.White; // Tab được chọn -> trắng
            btnDangXuat.ForeColor = Color.LightSkyBlue; // Chữ xanh

        }
    }
}
