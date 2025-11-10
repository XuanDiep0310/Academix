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

        private void UC_MyClasses_Load(object sender, EventArgs e)
        {
            LoadTeacherClasses();
        }

        private void LoadTeacherClasses()
        {
            flowPanelClasses.Controls.Clear();

            var card = new UC_ClassCard();
            var card1 = new UC_ClassCard();
            var card2 = new UC_ClassCard();
            var card3 = new UC_ClassCard();
            var card4 = new UC_ClassCard();
            var card5 = new UC_ClassCard();
            //var card = new UC_ClassCard(classId, name, code, studentCount);

            card.Margin = new Padding(10);
            flowPanelClasses.Controls.Add(card);
            flowPanelClasses.Controls.Add(card1);
            flowPanelClasses.Controls.Add(card2);
            flowPanelClasses.Controls.Add(card3);
            flowPanelClasses.Controls.Add(card4);
            flowPanelClasses.Controls.Add(card5);

        }
    }
}
