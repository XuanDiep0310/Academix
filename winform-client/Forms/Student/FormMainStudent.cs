using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms.Student.MyResult;
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

namespace Academix.WinApp.Forms.Student
{
    public partial class FormMainStudent : Form
    {
        public FormMainStudent()
        {
            InitializeComponent();
        }

        private void ResetTabButtons()
        {
            Color defaultColor = Color.LightSkyBlue; // Xanh dương mặc định
            Color defaultTextColor = Color.White;

            btnMyClass.FillColor = defaultColor;
            btnMyClass.ForeColor = defaultTextColor;

            btnTaiLieuHocTap.FillColor = defaultColor;
            btnTaiLieuHocTap.ForeColor = defaultTextColor;

            btnBaiKiemTra.FillColor = defaultColor;
            btnBaiKiemTra.ForeColor = defaultTextColor;

            btnMyResult.FillColor = defaultColor;
            btnMyResult.ForeColor = defaultTextColor;

            btnDoiMatKhau.FillColor = defaultColor;
            btnDoiMatKhau.ForeColor = defaultTextColor;

            btnDangXuat.FillColor = defaultColor;
            btnDangXuat.ForeColor = defaultTextColor;
        }

        private void btnMyClass_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnMyClass.FillColor = Color.White;
            btnMyClass.ForeColor = Color.LightSkyBlue;

            mainPanel.Controls.Clear();

            UC_LopHocCuaToi uc = new UC_LopHocCuaToi();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnTaiLieuHocTap_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnTaiLieuHocTap.FillColor = Color.White;
            btnTaiLieuHocTap.ForeColor = Color.LightSkyBlue;

            mainPanel.Controls.Clear();

            UC_MyMaterials uc = new UC_MyMaterials();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnBaiKiemTra_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnBaiKiemTra.FillColor = Color.White;
            btnBaiKiemTra.ForeColor = Color.LightSkyBlue;

            mainPanel.Controls.Clear();

            UC_MyExams uc = new UC_MyExams();

            uc.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(uc);
        }

        private void btnMyResult_Click(object sender, EventArgs e)
        {
            ResetTabButtons();

            btnMyResult.FillColor = Color.White;
            btnMyResult.ForeColor = Color.LightSkyBlue;

            mainPanel.Controls.Clear();
            UC_MyResult uc = new UC_MyResult();
            uc.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(uc);

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

                this.FindForm().Close();
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnDoiMatKhau.FillColor = Color.White; // Tab được chọn -> trắng
            btnDoiMatKhau.ForeColor = Color.LightSkyBlue; // Chữ xanh
            FormDoiMatKhau form = new FormDoiMatKhau();
            form.ShowDialog();
        }
    }
}
