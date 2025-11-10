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
    public partial class UC_QuestionCard : UserControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int QuestionId { get; set; }

        public UC_QuestionCard()
        {
            InitializeComponent();
        }

        public UC_QuestionCard(int questionId, string questionText, string subject, string difficultyLevel)
        {
            InitializeComponent();
            QuestionId = questionId;
        }


    }
}
