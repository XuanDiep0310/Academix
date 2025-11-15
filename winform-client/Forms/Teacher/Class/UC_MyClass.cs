using Academix.WinApp.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Academix.WinApp.Forms.Teacher
{
    public partial class UC_MyClasses : UserControl
    {


        public UC_MyClasses()
        {
            InitializeComponent();
        }
        private async void UC_MyClasses_Load(object sender, EventArgs e)
        {
            await LoadTeacherClasses();
        }

        private async Task LoadTeacherClasses()
        {
            var classApi = new ClassApiService();
            var classes = await classApi.GetMyClassesAsync();

            flowPanelClasses.Controls.Clear();

            foreach (var c in classes)
            {
                var card = new UC_ClassCard(c.ClassId, c.ClassName, c.ClassCode, c.StudentCount);
                card.Margin = new Padding(10);
                flowPanelClasses.Controls.Add(card);
            }
        }
    }
}
