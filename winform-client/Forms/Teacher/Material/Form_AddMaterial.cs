using Academix.WinApp.Api;
using Academix.WinApp.Models.Teacher;
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

namespace Academix.WinApp.Forms.Teacher.Material
{
    public partial class Form_AddMaterial : Form
    {
        public Form_AddMaterial()
        {
            InitializeComponent();
        }

        private async void Form_AddMaterial_Load(object sender, EventArgs e)
        {
            await Task.Delay(10); // tránh lỗi render WinForm
            LoadTypes();
            await LoadClasses();
        }

        private void LoadTypes()
        {
            cmbLoaiTaiLieu.Items.AddRange(new string[]
            {
        "PDF", "Video", "Image", "Link", "Other"
            });

            cmbLoaiTaiLieu.SelectedIndex = 0;
        }

        private async Task LoadClasses()
        {
            try
            {
                ClassApiService api = new ClassApiService();
                var myClasses = await api.GetMyClassesAsync();

                cmbLopHoc.DataSource = myClasses;
                cmbLopHoc.DisplayMember = "ClassName";
                cmbLopHoc.ValueMember = "ClassId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách lớp!\n" + ex.Message);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đóng không?", "Xác nhận",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.Close();
        }

        private async void btnThem_Click(object sender, EventArgs e)
        {
            if (cmbLopHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học!");
                return;
            }

            int classId = (int)cmbLopHoc.SelectedValue;

            string title = txtTeiuDe.Text.Trim();
            string desc = txtMoTa.Text.Trim();
            string type = cmbLoaiTaiLieu.SelectedItem.ToString();

            var api = new MaterialApiService();

            // Trường hợp upload file
            if (!string.IsNullOrEmpty(txtChonFile.Text))
            {
                var dto = new UploadMaterialRequestDto
                {
                    Title = title,
                    Description = desc,
                    FilePath = txtChonFile.Text
                };

                bool ok = await api.UploadMaterialFileAsync(classId, dto);

                if (ok)
                {
                    MessageBox.Show("Thêm tài liệu (upload) thành công!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Upload thất bại!");
                }

                return;
            }

            // Trường hợp tạo link
            if (!string.IsNullOrEmpty(txtURLTaiLieu.Text))
            {
                var dto = new CreateMaterialRequestDto
                {
                    Title = title,
                    Description = desc,
                    MaterialType = type,
                    FileUrl = txtURLTaiLieu.Text,
                    FileName = null,
                    FileSize = null
                };

                bool ok = await api.CreateMaterialAsync(classId, dto);

                if (ok)
                {
                    MessageBox.Show("Thêm tài liệu thành công!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tạo tài liệu thất bại!");
                }

                return;
            }

            MessageBox.Show("Hãy chọn file hoặc nhập URL!", "Thiếu dữ liệu");
        }


        private void txtChonFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtChonFile.Text = ofd.FileName;
            }
        }
    }
}
