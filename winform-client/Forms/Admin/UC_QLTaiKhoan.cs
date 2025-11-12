using Academix.WinApp.Api;
using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class UC_QLTaiKhoan : UserControl
    {
        private readonly UserApi _userApi;

        public UC_QLTaiKhoan()
        {
            InitializeComponent();

           
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));


            this.Load += UC_QLTaiKhoan_Load; // đảm bảo sự kiện load
        }

        private async void UC_QLTaiKhoan_Load(object sender, EventArgs e)
        {
            await LoadTaiKhoanAsync();
        }

        private async Task LoadTaiKhoanAsync()
        {
            try
            {
                var users = await _userApi.GetAllUsersAsync();
                Debug.WriteLine($"Users count: {users.Count}");

                if (users.Count > 0)
                {
                    dgvTaiKhoan.DataSource = users;

                    // Config columns
                    ConfigureColumns();

                    // Thêm cột thao tác (Edit, Lock, Delete)
                    AddActionColumns();
                }
                else
                {
                    dgvTaiKhoan.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadTaiKhoanAsync Exception: {ex}");
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureColumns()
        {
            // Ẩn UserId
            if (dgvTaiKhoan.Columns["UserId"] != null)
                dgvTaiKhoan.Columns["UserId"].Visible = false;

            // Set headers
            if (dgvTaiKhoan.Columns["FullName"] != null)
                dgvTaiKhoan.Columns["FullName"].HeaderText = "Họ tên";

            if (dgvTaiKhoan.Columns["Email"] != null)
                dgvTaiKhoan.Columns["Email"].HeaderText = "Email";

            if (dgvTaiKhoan.Columns["Role"] != null)
            {
                dgvTaiKhoan.Columns["Role"].HeaderText = "Vai trò";
                // Tô màu xanh cho Role
                dgvTaiKhoan.Columns["Role"].DefaultCellStyle.BackColor = Color.LightBlue;
                dgvTaiKhoan.Columns["Role"].DefaultCellStyle.ForeColor = Color.DarkBlue;
            }

            if (dgvTaiKhoan.Columns["IsActive"] != null)
            {
                dgvTaiKhoan.Columns["IsActive"].HeaderText = "Trạng thái";
                // Hiển thị "Hoạt động" thay vì True/False
            }

            if (dgvTaiKhoan.Columns["CreatedAt"] != null)
            {
                dgvTaiKhoan.Columns["CreatedAt"].HeaderText = "Ngày tạo";
                dgvTaiKhoan.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            // Ẩn UpdatedAt
            if (dgvTaiKhoan.Columns["UpdatedAt"] != null)
                dgvTaiKhoan.Columns["UpdatedAt"].Visible = false;

            // Set column order
            if (dgvTaiKhoan.Columns["FullName"] != null)
                dgvTaiKhoan.Columns["FullName"].DisplayIndex = 0;
            if (dgvTaiKhoan.Columns["Email"] != null)
                dgvTaiKhoan.Columns["Email"].DisplayIndex = 1;
            if (dgvTaiKhoan.Columns["Role"] != null)
                dgvTaiKhoan.Columns["Role"].DisplayIndex = 2;
            if (dgvTaiKhoan.Columns["IsActive"] != null)
                dgvTaiKhoan.Columns["IsActive"].DisplayIndex = 3;
            if (dgvTaiKhoan.Columns["CreatedAt"] != null)
                dgvTaiKhoan.Columns["CreatedAt"].DisplayIndex = 4;
        }

        private void AddActionColumns()
        {
            // Kiểm tra nếu chưa có cột thao tác
            if (dgvTaiKhoan.Columns["Actions"] == null)
            {
                // Thêm cột Edit
                DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn
                {
                    Name = "btnEdit",
                    HeaderText = "",
                    Text = "✏️",
                    UseColumnTextForButtonValue = true,
                    Width = 50
                };
                dgvTaiKhoan.Columns.Add(btnEdit);

                // Thêm cột Lock
                DataGridViewButtonColumn btnLock = new DataGridViewButtonColumn
                {
                    Name = "btnLock",
                    HeaderText = "",
                    Text = "🔒",
                    UseColumnTextForButtonValue = true,
                    Width = 50
                };
                dgvTaiKhoan.Columns.Add(btnLock);

                // Thêm cột Delete
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
                {
                    Name = "btnDelete",
                    HeaderText = "",
                    Text = "🗑️",
                    UseColumnTextForButtonValue = true,
                    Width = 50
                };
                dgvTaiKhoan.Columns.Add(btnDelete);
            }
        }


        private void btnThemGiaoVien_Click_1(object sender, EventArgs e)
        {
            using FormThemGiaoVien form = new FormThemGiaoVien();
            form.ShowDialog();
            // Load lại dữ liệu sau khi thêm
            _ = LoadTaiKhoanAsync();
        }

        private void btnThemHocSinh_Click(object sender, EventArgs e)
        {

        }



        //private void btnThemHocSinh_Click(object sender, EventArgs e)
        //{
        //    using FormThemHocSinh form = new FormThemHocSinh();
        //    form.ShowDialog();
        //    _ = LoadTaiKhoanAsync();
        //}
    }
}
