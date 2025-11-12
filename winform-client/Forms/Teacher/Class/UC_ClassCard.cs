using Academix.WinApp.Forms.Teacher.Class;
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
            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                // Panel nền mờ
                Panel overlayBg = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(100, 0, 0, 0), // mờ 50%
                    Name = "overlayBackground"
                };
                UC_ListStudent ucOverlay = new UC_ListStudent(ClassId);
                ucOverlay.Dock = DockStyle.Fill;

                // Thêm panel mờ + UC overlay
                parentForm.Controls.Add(overlayBg);
                parentForm.Controls.Add(ucOverlay);

                ucOverlay.BringToFront();
            }
        }
    }
}
