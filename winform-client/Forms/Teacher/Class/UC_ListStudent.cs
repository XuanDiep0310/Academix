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
        public UC_ListStudent()
        {
            InitializeComponent();
        }

        public UC_ListStudent(int classId)
        {
            InitializeComponent();
            this.ClassId = classId;

            LoadStudents();
        }

        private void LoadStudents()
        {
            // TODO: Viết code load dữ liệu theo classId ở đây
            // Ví dụ:
            // dgvStudents.DataSource = studentRepo.GetByClassId(classId);
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
    }
}
