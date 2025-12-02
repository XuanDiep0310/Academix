using Academix.WinApp.Api;
using Academix.WinApp.Models.Dashboard;
using Academix.WinApp.Utils;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class UC_TongQuanAdmin : UserControl
    {
        private readonly UserApi _userApi;

        public UC_TongQuanAdmin()
        {
            InitializeComponent();
            //this.Load += UC_TongQuanAdmin_Load;
            this.VisibleChanged += UC_TongQuanAdmin_VisibleChanged;

            // Khởi tạo UserApi với base URL của server
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));


        }

        private async void UC_TongQuanAdmin_Load_1(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                await LoadDashboardDataAsync();
            }
        }

        private async void UC_TongQuanAdmin_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.Parent != null)
            {
                await LoadDashboardDataAsync();
            }
        }

        public async Task RefreshDataAsync()
        {
            await LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Hiển thị loading indicator
                Cursor = Cursors.WaitCursor;

                // Gọi API để lấy thống kê người dùng
                try
                {
                    DashboardUserStatisticDto userDashboardData = await _userApi.GetUserDashboardAsync();

                    if (userDashboardData != null)
                    {
                        // Cập nhật UI thống kê người dùng
                        if (txtTongNguoiDung != null) txtTongNguoiDung.Text = userDashboardData.TotalUsers.ToString();
                        if (txtTongHocSinh != null) txtTongHocSinh.Text = userDashboardData.TotalStudents.ToString();
                        if (txtTongGiaoVien != null) txtTongGiaoVien.Text = userDashboardData.TotalTeachers.ToString();
                        if (txtKhongHoatDong != null) txtKhongHoatDong.Text = userDashboardData.InactiveUsers.ToString();
                        if (txtHoatDong != null) txtHoatDong.Text = userDashboardData.ActiveUsers.ToString();
                        if (txtSoAdmin != null) txtSoAdmin.Text = userDashboardData.TotalAdmins.ToString();

                        // Cập nhật dữ liệu tăng trưởng người dùng
                        if (userDashboardData.UserGrowth != null && userDashboardData.UserGrowth.Any())
                        {
                            var latestMonth = userDashboardData.UserGrowth
                                .OrderByDescending(x => x.Month)
                                .FirstOrDefault();

                            if (latestMonth != null)
                            {
                                if (NguoiDungMoi != null) NguoiDungMoi.Text = latestMonth.Count.ToString();
                                if (guna2HtmlLabel12 != null) guna2HtmlLabel12.Text = $"Người dùng mới ({latestMonth.Month})";
                            }
                            else
                            {
                                if (NguoiDungMoi != null) NguoiDungMoi.Text = "0";
                                if (guna2HtmlLabel12 != null) guna2HtmlLabel12.Text = "Người dùng mới";
                            }
                        }
                        else
                        {
                            if (NguoiDungMoi != null) NguoiDungMoi.Text = "0";
                            if (guna2HtmlLabel12 != null) guna2HtmlLabel12.Text = "Người dùng mới";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thống kê người dùng: {ex.Message}\n\nChi tiết: {ex.GetType().Name}",
                        "Lỗi thống kê người dùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Gọi API để lấy thống kê lớp học
                try
                {
                    ClassDashboardData classDashboardData = await _userApi.GetClassDashboardAsync();

                    if (classDashboardData != null)
                    {
                        // Cập nhật UI thống kê lớp học
                        if (lblTongSoLop != null) lblTongSoLop.Text = classDashboardData.TotalClasses.ToString();
                        if (lblLopHoatDong != null) lblLopHoatDong.Text = classDashboardData.ActiveClasses.ToString();
                        if (lblLopNgungHoatDong != null) lblLopNgungHoatDong.Text = classDashboardData.InactiveClasses.ToString();
                        if (lblTongSoHStrongLop != null) lblTongSoHStrongLop.Text = classDashboardData.TotalStudents.ToString();
                        //if (lblSiSoTB != null) lblSiSoTB.Text = classDashboardData.AverageStudentsPerClass.ToString("F1");

                        // Cập nhật lớp mới
                        if (classDashboardData.ClassGrowth != null && classDashboardData.ClassGrowth.Any())
                        {
                            var latestClassMonth = classDashboardData.ClassGrowth
                                .OrderByDescending(x => x.Month)
                                .FirstOrDefault();

                            if (latestClassMonth != null)
                            {
                                if (lblLopMoi != null) lblLopMoi.Text = latestClassMonth.Count.ToString();
                                if (guna2HtmlLabel18 != null) guna2HtmlLabel18.Text = $"Lớp mới ({latestClassMonth.Month})";
                            }
                            else
                            {
                                if (lblLopMoi != null) lblLopMoi.Text = "0";
                                if (guna2HtmlLabel18 != null) guna2HtmlLabel18.Text = "Lớp mới";
                            }
                        }
                        else
                        {
                            if (lblLopMoi != null) lblLopMoi.Text = "0";
                            if (guna2HtmlLabel18 != null) guna2HtmlLabel18.Text = "Lớp mới";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thống kê lớp học: {ex.Message}\n\nChi tiết: {ex.GetType().Name}",
                        "Lỗi thống kê lớp học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Gọi API để lấy thống kê tài liệu
                try
                {
                    MaterialDashboardData materialDashboardData = await _userApi.GetMaterialDashboardAsync();

                    if (materialDashboardData != null)
                    {
                        // Cập nhật UI thống kê tài liệu
                        if (guna2HtmlLabel5 != null) guna2HtmlLabel5.Text = "Thống kê tài liệu";
                        if (lblSoTaiLieu != null) lblSoTaiLieu.Text = materialDashboardData.TotalMaterials.ToString();

                        string storageText = !string.IsNullOrEmpty(materialDashboardData.TotalStorageUsedFormatted)
                            ? materialDashboardData.TotalStorageUsedFormatted
                            : materialDashboardData.TotalStorageUsed.ToString() + " bytes";
                        if (lblDungLuong != null) lblDungLuong.Text = storageText;

                        if (lblTaiLieuHomNay != null) lblTaiLieuHomNay.Text = materialDashboardData.MaterialsUploadedToday.ToString();
                        if (lblTaiLieuTuanNay != null) lblTaiLieuTuanNay.Text = materialDashboardData.MaterialsUploadedThisWeek.ToString();
                        if (lblTaiLieuThangNay != null) lblTaiLieuThangNay.Text = materialDashboardData.MaterialsUploadedThisMonth.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thống kê tài liệu: {ex.Message}\n\nChi tiết: {ex.GetType().Name}",
                        "Lỗi thống kê tài liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Lỗi không xác định khi tải dữ liệu dashboard.\n\n";
                errorMessage += $"Thông báo: {ex.Message}\n";
                errorMessage += $"Loại lỗi: {ex.GetType().Name}\n";

                if (ex.InnerException != null)
                {
                    errorMessage += $"Lỗi chi tiết: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        //private void UC_TongQuanAdmin_Load_1(object sender, EventArgs e)
        //{

        //}
    }
}
