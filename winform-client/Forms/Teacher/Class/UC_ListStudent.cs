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

namespace Academix.WinApp.Forms.Teacher.Class
{
    public partial class UC_ListStudent : UserControl
    {
        public int ClassId { get; private set; }

        public UC_ListStudent(int classId)
        {
            InitializeComponent();
            this.ClassId = classId;

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            var parent = this.Parent;
            var form = this.FindForm();

            if (form != null)
            {
                // Xóa lớp nền mờ
                var overlayBg = form.Controls.Find("overlayBackground", true).FirstOrDefault();
                if (overlayBg != null)
                    form.Controls.Remove(overlayBg);
            }

            // Xóa UC hiện tại
            parent.Controls.Remove(this);
            this.Dispose();
        }

        private async void UC_ListStudent_Load(object sender, EventArgs e)
        {
            await LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            try
            {
                var api = new ClassApiService();
                List<ClassStudentDto> students = await api.GetStudentsByClassAsync(ClassId);

                dgvListStudent.AutoGenerateColumns = true;
                dgvListStudent.DataSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách học viên:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
