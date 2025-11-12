using Academix.WinApp.Forms.Teacher.Material;
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
    public partial class UC_Materials : UserControl
    {
        public UC_Materials()
        {
            InitializeComponent();
        }

        private void btnThemTaiLieu_Click(object sender, EventArgs e)
        {
            Form_AddMaterial frm = new Form_AddMaterial();
            frm.ShowDialog();
        }
    }
}
