using Academix.WinApp.Api;
using Academix.WinApp.Models.Teacher;
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
    public partial class UC_MyMaterials : UserControl
    {
        private List<MyClassResponseDto> _myClasses = new();

        public UC_MyMaterials()
        {
            InitializeComponent();
        }

        private async void UC_MyMaterials_Load(object sender, EventArgs e)
        {
            InitializeMaterialTypeFilter();
            await LoadClassesAsync();
        }

        private void InitializeMaterialTypeFilter()
        {
            cmbLoaiTaiLieu.Items.Clear();
            cmbLoaiTaiLieu.Items.Add("Tất cả");
            cmbLoaiTaiLieu.Items.Add("PDF");
            cmbLoaiTaiLieu.Items.Add("Video");
            cmbLoaiTaiLieu.Items.Add("Image");
            cmbLoaiTaiLieu.Items.Add("Link");
            cmbLoaiTaiLieu.SelectedIndex = 0;

            cmbLoaiTaiLieu.SelectedIndexChanged -= cmbLoaiTaiLieu_SelectedIndexChanged;
            cmbLoaiTaiLieu.SelectedIndexChanged += cmbLoaiTaiLieu_SelectedIndexChanged;
        }

        private async void cmbLoaiTaiLieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLopHoc.SelectedValue is int classId)
            {
                await LoadMaterialsForClass(classId);
            }
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                var classApi = new ClassApiService();
                _myClasses = await classApi.GetMyClassesAsync();

                cmbLopHoc.DataSource = _myClasses;
                cmbLopHoc.DisplayMember = "ClassName";
                cmbLopHoc.ValueMember = "ClassId";

                cmbLopHoc.SelectedIndexChanged -= cmbLopHoc_SelectedIndexChanged;
                cmbLopHoc.SelectedIndexChanged += cmbLopHoc_SelectedIndexChanged;

                if (_myClasses.Any())
                {
                    await LoadMaterialsForClass(_myClasses.First().ClassId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải danh sách lớp.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async void cmbLopHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLopHoc.SelectedValue is int classId)
            {
                await LoadMaterialsForClass(classId);
            }
        }

        private async Task LoadMaterialsForClass(int classId)
        {
            try
            {
                // Lấy loại tài liệu đã chọn từ combobox
                string? typeFilter = null;
                if (cmbLoaiTaiLieu.SelectedItem is string selectedType && selectedType != "Tất cả")
                {
                    typeFilter = selectedType;
                }

                var materialApi = new MaterialApiService();
                var paged = await materialApi.GetMaterialsPagedAsync(
                    classId,
                    typeFilter: typeFilter,
                    page: 1,
                    pageSize: 50
                );

                flowpanelMaterial.Controls.Clear();

                foreach (var m in paged.Materials)
                {
                    var card = new UC_MaterialCard();
                    card.Bind(m);
                    card.Margin = new Padding(10);
                    flowpanelMaterial.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải tài liệu cho lớp.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}
