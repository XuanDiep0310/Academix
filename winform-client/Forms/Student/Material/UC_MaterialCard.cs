using Academix.WinApp.Api;
using Academix.WinApp.Forms.Student.Material;
using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student
{
    public partial class UC_MaterialCard : UserControl
    {
        private MaterialResponseDto? _material;

        public UC_MaterialCard()
        {
            InitializeComponent();
        }

        public void Bind(MaterialResponseDto material)
        {
            _material = material;

            lblTenTaiLieu.Text = material.Title;
            lblLopHoc.Text = material.ClassName;
            lblLoaiTaiLieu.Text = material.MaterialType;
            lblMoTa.Text = string.IsNullOrWhiteSpace(material.Description)
                ? "Không có mô tả"
                : material.Description;
            lblNgayDang.Text = $"Đăng ngày: {material.CreatedAt:dd/MM/yyyy HH:mm}";

            btnTaiVe.Click -= btnTaiVe_Click;
            btnTaiVe.Click += btnTaiVe_Click;

            btnXemTaiLieu.Click -= btnXemTaiLieu_Click;
            btnXemTaiLieu.Click += btnXemTaiLieu_Click;
        }

        private async void btnTaiVe_Click(object? sender, EventArgs e)
        {
            if (_material == null)
                return;

            try
            {
                var api = new MaterialApiService();
                var ok = await api.DownloadAsync(_material.ClassId, _material);

                if (!ok)
                {
                    MessageBox.Show(
                        "Không thể tải file. Vui lòng thử lại.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải file: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async void btnXemTaiLieu_Click(object? sender, EventArgs e)
        {
            if (_material == null)
                return;

            try
            {
                // Nếu có FileUrl, mở trực tiếp trên trình duyệt
                if (!string.IsNullOrWhiteSpace(_material.FileUrl))
                {
                    var url = _material.FileUrl;

                    // Nếu là relative URL, thêm base URL
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        var baseUrl = Config.GetApiBaseUrl().TrimEnd('/');
                        url = $"{baseUrl}/{url.TrimStart('/')}";
                    }

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                    return;
                }

                // Nếu không có FileUrl, tải về và mở
                var api = new MaterialApiService();
                var bytes = await api.DownloadBytesAsync(_material.ClassId, _material.MaterialId);

                var extension = Path.GetExtension(_material.FileName ?? string.Empty);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".tmp";
                }

                var tempFile = Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}{extension}"
                );

                await File.WriteAllBytesAsync(tempFile, bytes);

                // Mở file bằng ứng dụng mặc định
                Process.Start(new ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể xem tài liệu: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
