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
    public partial class UC_MyMaterials : UserControl
    {
        public UC_MyMaterials()
        {
            InitializeComponent();
        }

        private async void UC_MyMaterials_Load(object sender, EventArgs e)
        {
            await LoadMaterials();
        }

        private async Task LoadMaterials()
        {
            //var classApi = new ClassApiService();
            //var classes = await classApi.GetMyClassesAsync();

            //layoutMyCard.Controls.Clear();

            //foreach (var c in classes)
            //{
            //    var card = new UC_ClassCard(c.ClassId, c.ClassName, c.ClassCode);
            //    card.Margin = new Padding(10);
            //    layoutMyCard.Controls.Add(card);
            //}

            var card1 = new UC_MaterialCard();
            flowpanelMaterial.Controls.Add(card1);
            var card2 = new UC_MaterialCard();
            flowpanelMaterial.Controls.Add(card2);
        }

    }
}
