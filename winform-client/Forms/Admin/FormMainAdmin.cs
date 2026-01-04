using Academix.WinApp.Api;
using Academix.WinApp.Utils;
using Academix.WinApp.Forms;
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

namespace Academix.WinApp.Forms.Admin
{
    public partial class FormMainAdmin : Form
    {
        private readonly UserApi _userApi;
        private Guna.UI2.WinForms.Guna2Button btnLanguage;

        public FormMainAdmin()
        {
            InitializeComponent();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));
            ResetTabButtons();
            CreateLanguageButton();
           
        }

        private void CreateLanguageButton()
        {
            btnLanguage = new Guna.UI2.WinForms.Guna2Button();
            btnLanguage.BorderRadius = 5;
            btnLanguage.FillColor = Color.FromArgb(135, 206, 250);
            btnLanguage.Font = new Font("Segoe UI", 9F);
            btnLanguage.ForeColor = Color.White;
            btnLanguage.Location = new Point(guna2Panel2.Width - 90, 10);
            btnLanguage.Size = new Size(80, 30);
            btnLanguage.Text = LanguageManager.CurrentLanguage.ToUpper();
            //btnLanguage.Click += btnLanguage_Click;
            guna2Panel2.Controls.Add(btnLanguage);
            btnLanguage.BringToFront();
        }

        

        private void ResetTabButtons()
        {
            Color defaultColor = Color.LightSkyBlue; // Xanh dương mặc định
            Color defaultTextColor = Color.White;

            btnQLTaiKhoan.FillColor = defaultColor;
            btnQLTaiKhoan.ForeColor = defaultTextColor;

            btnQLLopHoc.FillColor = defaultColor;
            btnQLLopHoc.ForeColor = defaultTextColor;

            //btnCaiDat.FillColor = defaultColor;
            //btnCaiDat.ForeColor = defaultTextColor;

            btnTongQuan.FillColor = defaultColor;
            btnTongQuan.ForeColor = defaultTextColor;


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

        private void btnTongQuan_Click_1(object sender, EventArgs e)
        {
            ResetTabButtons();
            btnTongQuan.FillColor = Color.White; // Tab được chọn -> trắng
            btnTongQuan.ForeColor = Color.LightSkyBlue; // Chữ xanh
            mainPanel.Controls.Clear();
            UC_TongQuanAdmin uc = new UC_TongQuanAdmin();
            uc.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(uc);
        }

        //private void btnCaiDat_Click(object sender, EventArgs e)
        //{
        //    ResetTabButtons();
        //    btnCaiDat.FillColor = Color.White; // Tab được chọn -> trắng
        //    btnCaiDat.ForeColor = Color.LightSkyBlue; // Chữ xanh

        //    mainPanel.Controls.Clear();
        //    UC_CaiDat uc = new UC_CaiDat();
        //    uc.Dock = DockStyle.Fill;
        //    mainPanel.Controls.Add(uc);
        //}

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
            // Nếu chọn No thì không làm gì, form vẫn giữ nguyên
        }





        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void FormMainAdmin_Load(object sender, EventArgs e)
        {
            // Load ngôn ngữ khi form load
            //LoadLanguage();
            
            // Đảm bảo tất cả button được cập nhật
            RefreshAllButtons();
        }

        private void RefreshAllButtons()
        {
            // Force refresh tất cả button
            btnTongQuan.Invalidate();
            btnQLTaiKhoan.Invalidate();
            btnQLLopHoc.Invalidate();
            btnDoiMatKhau.Invalidate();
            btnDangXuat.Invalidate();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Hủy đăng ký event khi form đóng
            //LanguageManager.LanguageChanged -= LanguageManager_LanguageChanged;
            base.OnFormClosed(e);
        }
    }
}
