using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Exam;
using Academix.WinApp.Forms.Teacher.Question;
using Academix.WinApp.Models.Teacher;
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
    public partial class UC_Exams : UserControl
    {
        private int _classId;
        private int _page = 1;
        private int _totalPages = 1;

        public UC_Exams()
        {
            InitializeComponent();
        }
        private async void UC_Exams_Load(object sender, EventArgs e)
        {
            await LoadClassesAsync();
            await LoadExamsAsync();
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
                {
                    cmbLopHoc.SelectedIndex = 0;
                    _classId = myClasses[0].ClassId;
                }

                cmbLopHoc.SelectedIndexChanged += async (s, e) =>
                {
                    _classId = (int)cmbLopHoc.SelectedValue;
                    _page = 1;
                    await LoadExamsAsync();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh sách lớp: " + ex.Message);
            }
        }

        private async Task LoadExamsAsync()
        {
            try
            {
                if (_classId <= 0)
                    return;

                flowPanelExams.Controls.Clear();

                var api = new ExamApiService();

                var result = await api.GetExamsByClassAsync(
                        classId: _classId,
                        page: _page
                    );

                if (result?.Data?.Exams == null || result.Data.Exams.Count == 0)
                {
                    flowPanelExams.Controls.Add(new Label()
                    {
                        Text = "Không có bài kiểm tra nào!",
                        AutoSize = true,
                        ForeColor = Color.Gray,
                        Font = new Font("Segoe UI", 11, FontStyle.Italic)
                    });
                    return;
                }

                _totalPages = result.Data.TotalPages;

                // Add vào flow panel
                foreach (var exam in result.Data.Exams)
                {
                    var examResponse = new ExamResponseDto
                    {
                        ExamId = exam.ExamId,
                        ClassId = exam.ClassId,
                        ClassName = exam.ClassName,
                        Title = exam.Title,
                        Description = exam.Description,
                        Duration = exam.Duration,
                        TotalMarks = exam.TotalMarks,
                        StartTime = exam.StartTime,
                        EndTime = exam.EndTime,
                        IsPublished = exam.IsPublished,
                        QuestionCount = exam.QuestionCount,
                        CreatedBy = exam.CreatedBy
                    };

                    var card = new UC_ExamCard(examResponse);
                    card.Width = flowPanelExams.ClientSize.Width - 20;

                    card.OnUpdated += async () => await LoadExamsAsync();
                    card.Margin = new Padding(10);
                    flowPanelExams.Controls.Add(card);
                }


                BuildPaginationUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load bài kiểm tra: " + ex.Message);
            }
        }



        private void btnTaoBaiKiemTra_Click(object sender, EventArgs e)
        {
            var frm = new Form_AddUpdateExam(_classId);
            frm.OnSaved += async () => await LoadExamsAsync();
            frm.ShowDialog();
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
                    await LoadExamsAsync();
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
                    await LoadExamsAsync();
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
                    await LoadExamsAsync();
                }
            };
            flowpnlBottom.Controls.Add(btnNext);
            
            // Căn giữa pagination
            flowpnlBottom.AutoSize = true;
            flowpnlBottom.Location = new Point((guna2Panel1.Width - flowpnlBottom.Width) / 2, (guna2Panel1.Height - flowpnlBottom.Height) / 2);
        }

        private void flowPanelExams_SizeChanged(object sender, EventArgs e)
        {
            ResizeCards();
        }
        private void ResizeCards()
        {
            foreach (Control c in flowPanelExams.Controls)
            {
                c.Width = flowPanelExams.ClientSize.Width - 20; // trừ margin
            }
        }

    }
}
