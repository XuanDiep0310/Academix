using Academix.WinApp.Api;
using Academix.WinApp.Models.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin.ClassManagement
{
    public partial class FormAddTeacherClass : Form
    {
        private readonly int _classId;
        private readonly ClassApiService _classApi;
        private List<ClassMember> _allStudents;

        public FormAddTeacherClass(int classId)
        {
            InitializeComponent();
            _classId = classId;
            //_classApi = new ClassApiService("YOUR_BEARER_TOKEN"); // gán token

            //this.Load += FormAddStudentClass_Load;
        }
        public FormAddTeacherClass()
        {
            InitializeComponent();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {

        }

        private void btnHuy_Click(object sender, EventArgs e)
        {

        }
    }
}
