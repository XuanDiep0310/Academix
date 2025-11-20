using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Material;
using Academix.WinApp.Forms.Teacher.Question;
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
    public partial class UC_Questions : UserControl
    {
        private int _page = 1;
        private int _pageSize = 6;
        private int _totalPages = 1;

        public UC_Questions()
        {
            InitializeComponent();
        }

        private async void UC_Questions_Load(object sender, EventArgs e)
        {
            await LoadQuestionsAsync();
            await LoadClassesAsync();
        }

        private async Task LoadQuestionsAsync()
        {
            flowPanelQuestion.Controls.Clear();

            var api = new QuestionApiService();
            var result = await api.GetMyQuestionsPagedAsync();

            if (result?.Data?.Questions == null || result.Data.Questions.Count == 0)
            {
                MessageBox.Show("Không có câu hỏi nào !");
                return;
            }

            _totalPages = result.Data.TotalPages;

            foreach (var item in result.Data.Questions)
            {
                var card = new UC_QuestionCard(item);

                // Đăng ký callback event để form reload sau khi sửa/xóa
                card.OnUpdated += async () => await LoadQuestionsAsync();

                card.Margin = new Padding(10);
                flowPanelQuestion.Controls.Add(card);
            }

            BuildPaginationUI();
        }




        private async Task LoadClassesAsync()
        {
            ClassApiService api = new ClassApiService();
            var myClasses = await api.GetMyClassesAsync();

            cmbLopHoc.DataSource = myClasses;
            cmbLopHoc.DisplayMember = "ClassName";
            cmbLopHoc.ValueMember = "ClassId";
            if (myClasses.Count > 0)
                cmbLopHoc.SelectedIndex = 0;
        }

        private async void btnThemCauHoi_Click(object sender, EventArgs e)
        {
            using (var frm = new Form_AddUpdateQuestion())
            {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    await LoadQuestionsAsync(); 
                }
            }
        }

        private void BuildPaginationUI()
        {
            flowpnlBottom.Controls.Clear();

            // Prev button
            var btnPrev = new Button
            {
                Text = "Prev",
                Height = 40,
                Enabled = _page > 1
            };
            btnPrev.Click += (s, e) => { _page--; _ = LoadQuestionsAsync(); };
            flowpnlBottom.Controls.Add(btnPrev);

            // Page numbers
            for (int i = 1; i <= _totalPages; i++)
            {
                var btnPage = new Button
                {
                    Text = i.ToString(),
                    Width = 35,
                    Height = 40,
                    BackColor = (i == _page) ? Color.LightSkyBlue : Color.White
                };
                int pageNum = i;
                btnPage.Click += (s, e) => { _page = pageNum; _ = LoadQuestionsAsync(); };
                flowpnlBottom.Controls.Add(btnPage);
            }

            // Next button
            var btnNext = new Button
            {
                Text = "Next",
                Height = 40,
                Enabled = _page < _totalPages
            };
            btnNext.Click += (s, e) => { _page++; _ = LoadQuestionsAsync(); };
            flowpnlBottom.Controls.Add(btnNext);
        }

    }
}
