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

        private async void btnImportExcel_Click(object sender, EventArgs e)
        {
            using var frm = new Form_ImportQuestions();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _page = 1; // quay về trang đầu sau khi import
                await LoadQuestionsAsync();
            }
        }

        private void BuildPaginationUI()
        {
            flowpnlBottom.Controls.Clear();

            // Prev button với Guna2Button
            var btnPrev = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "◀ Trước",
                Width = 90,
                Height = 42,
                BorderRadius = 20,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(200, 200, 200),
                FillColor = Color.White,
                ForeColor = Color.FromArgb(70, 130, 180),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Enabled = _page > 1,
                Margin = new Padding(5, 0, 5, 0)
            };
            btnPrev.Click += async (s, e) =>
            {
                if (_page > 1)
                {
                    _page--;
                    await LoadQuestionsAsync();
                }
            };
            flowpnlBottom.Controls.Add(btnPrev);

            // Page numbers với Guna2Button - chỉ hiển thị tối đa 5 trang
            int maxPagesToShow = 5;
            int start = Math.Max(1, _page - 2);
            int end = Math.Min(_totalPages, start + maxPagesToShow - 1);
            
            for (int i = start; i <= end; i++)
            {
                var btnPage = new Guna.UI2.WinForms.Guna2Button
                {
                    Text = i.ToString(),
                    Width = 45,
                    Height = 42,
                    BorderRadius = 20,
                    BorderThickness = 1,
                    FillColor = (i == _page) ? Color.FromArgb(70, 130, 180) : Color.White,
                    ForeColor = (i == _page) ? Color.White : Color.FromArgb(70, 130, 180),
                    BorderColor = (i == _page) ? Color.FromArgb(70, 130, 180) : Color.FromArgb(200, 200, 200),
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(3, 0, 3, 0)
                };
                int pageNum = i;
                btnPage.Click += async (s, e) =>
                {
                    _page = pageNum;
                    await LoadQuestionsAsync();
                };
                flowpnlBottom.Controls.Add(btnPage);
            }

            // Next button với Guna2Button
            var btnNext = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Sau ▶",
                Width = 90,
                Height = 42,
                BorderRadius = 20,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(200, 200, 200),
                FillColor = Color.White,
                ForeColor = Color.FromArgb(70, 130, 180),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Enabled = _page < _totalPages,
                Margin = new Padding(5, 0, 5, 0)
            };
            btnNext.Click += async (s, e) =>
            {
                if (_page < _totalPages)
                {
                    _page++;
                    await LoadQuestionsAsync();
                }
            };
            flowpnlBottom.Controls.Add(btnNext);
            
            // Căn giữa pagination
            flowpnlBottom.AutoSize = true;
            var parentPanel = flowpnlBottom.Parent as Guna.UI2.WinForms.Guna2Panel;
            if (parentPanel != null)
            {
                flowpnlBottom.Location = new Point((parentPanel.Width - flowpnlBottom.Width) / 2, (parentPanel.Height - flowpnlBottom.Height) / 2);
            }
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
