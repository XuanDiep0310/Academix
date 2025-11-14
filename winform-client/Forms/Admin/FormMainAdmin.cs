using Academix.WinApp.Api;
using Academix.WinApp.Utils;
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
        private readonly UserApi _userApi;
        public FormMainAdmin()
        {
            InitializeComponent();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));
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
            // Hiển thị hộp thoại xác nhận
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Xóa session
                SessionManager.ClearSession();

                // Reset màu tab
                ResetTabButtons();
                btnDangXuat.FillColor = Color.White;
                btnDangXuat.ForeColor = Color.LightSkyBlue;

                // Ẩn form hiện tại và mở FormSignIn
                this.FindForm().Hide();
                using var loginForm = new FormSignIn();
                loginForm.ShowDialog();

                // Đóng form hiện tại sau khi FormSignIn đóng
                this.FindForm().Close();
            }
            // Nếu chọn No thì không làm gì, form vẫn giữ nguyên
        }





        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void FormMainAdmin_Load(object sender, EventArgs e)
        {

        }
    }
}
