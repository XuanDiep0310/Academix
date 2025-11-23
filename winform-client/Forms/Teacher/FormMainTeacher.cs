using Academix.WinApp.Forms.Admin;
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

namespace Academix.WinApp.Forms.Teacher
{
    public partial class FormMainTeacher : Form
    {

        private UserControl currentControl = null;

        public FormMainTeacher()
        {
            InitializeComponent();
            lblHoTen.Text = SessionManager.CurrentUser.FullName;

        }

        private void ResetTabButtons()
        {
            // Màu pastel mềm mại hơn
            Color defaultColor = Color.FromArgb(135, 206, 250); // LightSkyBlue nhạt hơn
            Color defaultTextColor = Color.White;

            btnLopHocCuaToi.FillColor = defaultColor;
            btnLopHocCuaToi.ForeColor = defaultTextColor;

            btnTaiLieu.FillColor = defaultColor;
            btnTaiLieu.ForeColor = defaultTextColor;

            btnNganHangCauHoi.FillColor = defaultColor;
            btnNganHangCauHoi.ForeColor = defaultTextColor;

            btnBaiKiemTra.FillColor = defaultColor;
            btnBaiKiemTra.ForeColor = defaultTextColor;

            btnKetQua.FillColor = defaultColor;
            btnKetQua.ForeColor = defaultTextColor;

            btnDoiMatKhau.FillColor = defaultColor;
            btnDoiMatKhau.ForeColor = defaultTextColor;

            btnDangXuat.FillColor = defaultColor;
            btnDangXuat.ForeColor = defaultTextColor;
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
            ResetTabButtons();

            btnLopHocCuaToi.FillColor = Color.White;
            btnLopHocCuaToi.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue
            ShowUserControl(new UC_MyClasses());
        }



        private void btnTaiLieu_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnTaiLieu.FillColor = Color.White;
            btnTaiLieu.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue
            ShowUserControl(new UC_Materials());
        }

        private void btnNganHangCauHoi_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnNganHangCauHoi.FillColor = Color.White;
            btnNganHangCauHoi.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue
            ShowUserControl(new UC_Questions());
        }

        private void btnBaiKiemTra_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnBaiKiemTra.FillColor = Color.White;
            btnBaiKiemTra.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue
            ShowUserControl(new UC_Exams());
        }

        private void btnKetQua_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnKetQua.FillColor = Color.White;
            btnKetQua.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue
            ShowUserControl(new UC_Result());
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
                btnDangXuat.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue

                // Ẩn form hiện tại và mở FormSignIn
                this.FindForm().Hide();
                using var loginForm = new FormSignIn();
                loginForm.ShowDialog();

                this.FindForm().Close();
            }        
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnDoiMatKhau.FillColor = Color.White; // Tab được chọn -> trắng
            btnDoiMatKhau.ForeColor = Color.FromArgb(70, 130, 180); // SteelBlue
            FormDoiMatKhau form = new FormDoiMatKhau();
            form.ShowDialog();

        }
    }
}
