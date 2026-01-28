using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms;
using Academix.WinApp.Forms.Student.MyResult;
using Academix.WinApp.Utils;
using Academix.WinApp.Languages;
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
        private Guna.UI2.WinForms.Guna2Button btnLanguage;

        public FormMainStudent()
        {
            InitializeComponent();

            // Kiểm tra session trước khi truy cập
            if (SessionManager.CurrentUser != null)
            {
                lblTenHocSinh.Text = SessionManager.CurrentUser.FullName ?? LanguageManager.GetString("Student");
            }
            else
            {
                MessageBox.Show(
                    LanguageManager.GetString("InvalidSession"),
                    LanguageManager.GetString("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }

            ResetTabButtons();

        }



        private void ResetTabButtons()
        {
            Color defaultColor = Color.LightSkyBlue; // Indigo mặc định
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
                LanguageManager.GetString("ConfirmLogout"),
                LanguageManager.GetString("LogoutTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Xóa session
                SessionManager.ClearSession();

                // Tìm FormSignIn trong Application.OpenForms hoặc tạo mới
                FormSignIn loginForm = null;
                foreach (Form form in Application.OpenForms)
                {
                    if (form is FormSignIn)
                    {
                        loginForm = form as FormSignIn;
                        break;
                    }
                }

                // Nếu không tìm thấy FormSignIn, tạo mới
                if (loginForm == null || loginForm.IsDisposed)
                {
                    loginForm = new FormSignIn();
                }

                // Hiển thị lại FormSignIn và reset thông tin
                loginForm.ShowLoginForm();

                // Đóng form hiện tại
                this.Close();
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnDoiMatKhau.FillColor = Color.White; // Tab được chọn -> trắng
            btnDoiMatKhau.ForeColor = Color.LightSkyBlue; // Chữ indigo
            FormDoiMatKhau form = new FormDoiMatKhau();
            form.ShowDialog();
        }

        private void FormMainStudent_Load(object sender, EventArgs e)
        {
            // Load ngôn ngữ khi form load
            //LoadLanguage();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Hủy đăng ký event khi form đóng
            //LanguageManager.LanguageChanged -= LanguageManager_LanguageChanged;
            base.OnFormClosed(e);
        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
