using Academix.WinApp.Api;
using Academix.WinApp.Forms.Admin;
using Academix.WinApp.Forms.Teacher;
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
    public partial class UC_LopHocCuaToi : UserControl
    {
        public UC_LopHocCuaToi()
        {
            InitializeComponent();
        }

        private async void UC_LopHocCuaToi_Load(object sender, EventArgs e)
        {
            await LoadStudentClasses();
        }

        private async Task LoadStudentClasses()
        {
            try
            {
                var classApi = new ClassApiService();
                var classes = await classApi.GetMyClassesAsync();

                layoutMyCard.Controls.Clear();

                foreach (var c in classes)
                {
                    var card = new UC_MyClassCard(c.ClassId, c.ClassName, c.ClassCode);
                    card.Margin = new Padding(10);
                    layoutMyCard.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải danh sách lớp của bạn.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
