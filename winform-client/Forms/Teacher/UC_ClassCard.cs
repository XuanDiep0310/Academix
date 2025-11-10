using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher
{
    public partial class UC_ClassCard : UserControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ClassId { get; set; }

        public UC_ClassCard()
        {
            InitializeComponent();
        }

        public UC_ClassCard(int classId, string className, string classCode, int studentCount)
        {
            InitializeComponent();
            ClassId = classId;
            lblTenLop.Text = className;
            lblMaLop.Text = classCode;
            lblSoLuong.Text = $"{studentCount} học viên";
        }


        private void btnXemDSHS_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Xem chi tiết lớp: {lblTenLop.Text} (ID: {ClassId})");
            // Có thể mở form khác ở đây, ví dụ: FormClassDetails
        }
    }
}
