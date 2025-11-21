using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Material;
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
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher
{
    public partial class UC_Materials : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;

        public UC_Materials()
        {
            InitializeComponent();
        }

        private void UC_Materials_Load(object sender, EventArgs e)
        {
            LoadLoaiTaiLieu();
            LoadClassesAsync();
            LoadTaiLieuAsync();
        }

        private async Task LoadClassesAsync()
        {
            ClassApiService api = new ClassApiService();
            var myClasses = await api.GetMyClassesAsync();

            cmbLopHoc.DataSource = myClasses;
            cmbLopHoc.DisplayMember = "ClassName";
            cmbLopHoc.ValueMember = "ClassId";
            if (myClasses.Count > 0)
                cmbLopHoc.SelectedIndex = 0;
        }
        private void LoadLoaiTaiLieu()
        {
            cmbLoaiTaiLieu.Items.Clear();
            cmbLoaiTaiLieu.Items.Add("Tất cả");
            cmbLoaiTaiLieu.Items.Add("PDF");
            cmbLoaiTaiLieu.Items.Add("Video");
            cmbLoaiTaiLieu.Items.Add("Image");
            cmbLoaiTaiLieu.Items.Add("Link");
            cmbLoaiTaiLieu.Items.Add("Other");
            cmbLoaiTaiLieu.SelectedIndex = 0;
        }


        private async Task LoadTaiLieuAsync()
        {
            if (cmbLopHoc.SelectedItem == null)
                return;

            var selectedClass = cmbLopHoc.SelectedItem as MyClassResponseDto;
            if (selectedClass == null)
                return;

            int classId = selectedClass.ClassId;

            string? type = cmbLoaiTaiLieu.SelectedIndex > 0
                ? cmbLoaiTaiLieu.SelectedItem?.ToString()
                : null;

            MaterialApiService api = new MaterialApiService();
            //var materials = await api.GetMaterialsAsync(classId, type);

            var paged = await api.GetMaterialsPagedAsync(
                classId,
                type,
                currentPage,
                pageSize
            );

            totalPages = paged.TotalPages;


            dgvTaiLieu.DataSource = null;
            dgvTaiLieu.AutoGenerateColumns = false;

            // Thêm cột dữ liệu 1 lần
            if (dgvTaiLieu.Columns["MaterialId"] == null)
            {
                dgvTaiLieu.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaterialId", DataPropertyName = "MaterialId", HeaderText = "ID" });
                dgvTaiLieu.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Tiêu đề" });
                dgvTaiLieu.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", DataPropertyName = "Description", HeaderText = "Mô tả" });
                dgvTaiLieu.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaterialType", DataPropertyName = "MaterialType", HeaderText = "Loại" });
                dgvTaiLieu.Columns.Add(new DataGridViewTextBoxColumn { Name = "FileName", DataPropertyName = "FileName", HeaderText = "Tên file" });
                dgvTaiLieu.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreatedAt", DataPropertyName = "CreatedAt", HeaderText = "Ngày tạo" });

                AddActionButtons();
            }

            dgvTaiLieu.DataSource = paged.Materials;

            //RenderPaginationButtons();

            BuildPaginationUI();


        }



        private void btnThemTaiLieu_Click(object sender, EventArgs e)
        {
            Form_AddMaterial frm = new Form_AddMaterial();
            frm.ShowDialog();
            LoadTaiLieuAsync();
        }

        private async void cmbLopHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            await LoadTaiLieuAsync();
        }

        private async void cmbLoaiTaiLieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            await LoadTaiLieuAsync();
        }
        private void AddActionButtons()
        {
            // Xem chi tiết
            var btnView = new DataGridViewButtonColumn();
            btnView.Name = "btnView";
            btnView.HeaderText = "";
            btnView.Text = "Xem";
            btnView.UseColumnTextForButtonValue = true;
            dgvTaiLieu.Columns.Add(btnView);

            // Xóa
            var btnDelete = new DataGridViewButtonColumn();
            btnDelete.Name = "btnDelete";
            btnDelete.HeaderText = "";
            btnDelete.Text = "Xóa";
            btnDelete.UseColumnTextForButtonValue = true;
            dgvTaiLieu.Columns.Add(btnDelete);

            // Download
            var btnDownload = new DataGridViewButtonColumn();
            btnDownload.Name = "btnDownload";
            btnDownload.HeaderText = "";
            btnDownload.Text = "Tải xuống";
            btnDownload.UseColumnTextForButtonValue = true;
            dgvTaiLieu.Columns.Add(btnDownload);
        }

        private async void dgvTaiLieu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var material = (MaterialResponseDto)dgvTaiLieu.Rows[e.RowIndex].DataBoundItem;

            string columnName = dgvTaiLieu.Columns[e.ColumnIndex].Name;

            switch (columnName)
            {
                case "btnView":
                    ShowMaterialDetails(material);
                    break;

                case "btnDelete":
                    await DeleteMaterial(material);
                    break;

                case "btnDownload":
                    await DownloadMaterial(material);
                    break;
            }
        }
        private async void ShowMaterialDetails(MaterialResponseDto m)
        {
            if (m == null)
                return;

            try
            {
                // Nếu có FileUrl, mở trực tiếp trên trình duyệt
                if (!string.IsNullOrWhiteSpace(m.FileUrl))
                {
                    var url = m.FileUrl;

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
                var bytes = await api.DownloadBytesAsync(m.ClassId, m.MaterialId);

                var extension = Path.GetExtension(m.FileName ?? string.Empty);
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


        private async Task DeleteMaterial(MaterialResponseDto m)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa tài liệu này?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            MaterialApiService api = new MaterialApiService();
            bool ok = await api.DeleteMaterialAsync(m.ClassId, m.MaterialId);

            if (ok)
            {
                MessageBox.Show("Xóa thành công!", "Thông báo");
                await LoadTaiLieuAsync();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!", "Lỗi");
            }
        }
        private async Task DownloadMaterial(MaterialResponseDto m)
        {
            if (string.IsNullOrWhiteSpace(m.MaterialType))
                return;

            switch (m.MaterialType.ToLower())
            {
                case "link":
                    // Nếu là link, mở trực tiếp trong trình duyệt
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = m.FileUrl,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể mở link: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                default:
                    // Các loại khác: PDF, Video, Image, Other -> tải về
                    MaterialApiService api = new MaterialApiService();
                    bool ok = await api.DownloadAsync(m.ClassId, m);

                    if (ok)
                        MessageBox.Show("Tải xuống thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không thể tải file!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }


        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                await LoadTaiLieuAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                await LoadTaiLieuAsync();
            }
        }

        private Guna.UI2.WinForms.Guna2Button CreatePageButton(string text)
        {
            return new Guna.UI2.WinForms.Guna2Button
            {
                Text = text,
                Width = 45,
                Height = 40,
                BorderRadius = 15,
                BorderThickness = 1,
                BorderColor = Color.Gray,
                FillColor = Color.White,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
        }

        private void RenderPaginationButtons()
        {
            pnlPagination.Controls.Clear();

            int x = 0;

            // Prev button
            var btnPrev = CreatePageButton("Prev");
            btnPrev.Enabled = currentPage > 1;
            btnPrev.Location = new Point(x, 0);
            btnPrev.Click += (s, e) =>
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    LoadTaiLieuAsync();
                }
            };
            pnlPagination.Controls.Add(btnPrev);
            x += btnPrev.Width + 5;

            // Show max 5 page numbers
            int maxPages = 5;
            int start = Math.Max(1, currentPage - 2);
            int end = Math.Min(totalPages, start + maxPages - 1);

            for (int i = start; i <= end; i++)
            {
                var btnPage = CreatePageButton(i.ToString());

                // highlight current page
                if (i == currentPage)
                {
                    btnPage.FillColor = Color.RoyalBlue;
                    btnPage.ForeColor = Color.White;
                }

                btnPage.Location = new Point(x, 0);
                pnlPagination.Controls.Add(btnPage);

                int page = i;
                btnPage.Click += (s, e) =>
                {
                    currentPage = page;
                    LoadTaiLieuAsync();
                };

                x += btnPage.Width + 5;
            }

            // Next button
            var btnNext = CreatePageButton("Next");
            btnNext.Enabled = currentPage < totalPages;
            btnNext.Location = new Point(x, 0);
            btnNext.Click += (s, e) =>
            {
                if (currentPage < totalPages)
                {
                    currentPage++;
                    LoadTaiLieuAsync();
                }
            };
            pnlPagination.Controls.Add(btnNext);
        }


    private void BuildPaginationUI()
        {
            pnlPagination.Controls.Clear(); // Hoặc flowpnlBottom nếu bạn đổi tên panel

            // Prev button
            var btnPrev = new Button
            {
                Text = "Prev",
                Height = 40,
                Enabled = currentPage > 1
            };
            btnPrev.Click += async (s, e) =>
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    await LoadTaiLieuAsync();
                }
            };
            pnlPagination.Controls.Add(btnPrev);

            // Page numbers
            for (int i = 1; i <= totalPages; i++)
            {
                var btnPage = new Button
                {
                    Text = i.ToString(),
                    Width = 35,
                    Height = 40,
                    BackColor = (i == currentPage) ? Color.LightSkyBlue : Color.White
                };
                int pageNum = i; // tránh closure
                btnPage.Click += async (s, e) =>
                {
                    currentPage = pageNum;
                    await LoadTaiLieuAsync();
                };
                pnlPagination.Controls.Add(btnPage);
            }

            // Next button
            var btnNext = new Button
            {
                Text = "Next",
                Height = 40,
                Enabled = currentPage < totalPages
            };
            btnNext.Click += async (s, e) =>
            {
                if (currentPage < totalPages)
                {
                    currentPage++;
                    await LoadTaiLieuAsync();
                }
            };
            pnlPagination.Controls.Add(btnNext);
        }
    }
}
