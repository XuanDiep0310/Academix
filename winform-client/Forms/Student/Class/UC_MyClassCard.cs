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

namespace Academix.WinApp.Forms.Student
{
    public partial class UC_MyClassCard : UserControl
    {

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ClassId { get; set; }

        public UC_MyClassCard()
        {
        }

        public UC_MyClassCard(int classId, string className, string classCode)
        {
            InitializeComponent();
            ClassId = classId;
            lblTenLop.Text = className;
            lblMaLop.Text = classCode;
        }

    }
}
