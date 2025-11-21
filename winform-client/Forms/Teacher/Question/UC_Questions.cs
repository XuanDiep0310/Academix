using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Question;
using Academix.WinApp.Models.Teacher;
using System;
using System.Drawing;
using System.Linq;
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
                var lblEmpty = new Label
                {
                    Text = "Không có câu hỏi nào!",
                    AutoSize = true,
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 11, FontStyle.Italic)
                };
                flowPanelQuestion.Controls.Add(lblEmpty);
                _totalPages = 1;
                BuildPaginationUI();
                return;
            }

            _totalPages = result.Data.TotalPages;

            foreach (var item in result.Data.Questions)
            {
                var card = new UC_QuestionCard(item)
                {
                    Width = flowPanelQuestion.ClientSize.Width - 20,
                    Margin = new Padding(10)
                };

                // Chỉ subscribe 1 lần
                card.OnUpdated += async () => await LoadQuestionsAsync();

                flowPanelQuestion.Controls.Add(card);
            }

            BuildPaginationUI();
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                ClassApiService api = new ClassApiService();
                var myClasses = await api.GetMyClassesAsync();

                cmbLopHoc.DataSource = myClasses;
                cmbLopHoc.DisplayMember = "ClassName";
                cmbLopHoc.ValueMember = "ClassId";

                if (myClasses.Count > 0)
                    cmbLopHoc.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh sách lớp: " + ex.Message);
            }
        }

        private async void btnThemCauHoi_Click(object sender, EventArgs e)
        {
            using var frm = new Form_AddUpdateQuestion();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _page = 1; // quay về trang đầu sau khi thêm
                await LoadQuestionsAsync();
            }
        }

        private void BuildPaginationUI()
        {
            flowpnlBottom.Controls.Clear();

            // Nút Trước
            var btnPrev = new Button
            {
                Text = "Trước",
                Height = 40,
                Enabled = _page > 1
            };
            btnPrev.Click += async (s, e) =>
            {
                _page--;
                await LoadQuestionsAsync();
            };
            flowpnlBottom.Controls.Add(btnPrev);

            // Nút số trang
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
                btnPage.Click += async (s, e) =>
                {
                    _page = pageNum;
                    await LoadQuestionsAsync();
                };
                flowpnlBottom.Controls.Add(btnPage);
            }

            // Nút Tiếp
            var btnNext = new Button
            {
                Text = "Sau",
                Height = 40,
                Enabled = _page < _totalPages
            };
            btnNext.Click += async (s, e) =>
            {
                _page++;
                await LoadQuestionsAsync();
            };
            flowpnlBottom.Controls.Add(btnNext);
        }

        private void flowPanelQuestion_SizeChanged(object sender, EventArgs e)
        {
            ResizeCards();
        }

        private void ResizeCards()
        {
            foreach (Control c in flowPanelQuestion.Controls)
            {
                c.Width = flowPanelQuestion.ClientSize.Width - 20; // trừ margin
            }
        }
    }
}
