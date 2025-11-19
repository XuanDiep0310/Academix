using Academix.WinApp.Api;
using Academix.WinApp.Forms.Teacher.Exam;
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
                        isPublished: null,
                        page: _page,
                        pageSize: 6,
                        sortBy: "CreatedAt",
                        sortOrder: "desc"
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
                    var card = new UC_ExamCard(exam);   // Dùng ExamCard, KHÔNG phải QuestionCard

                    // Khi exam được cập nhật -> reload lại danh sách
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

            // Prev
            var btnPrev = new Button
            {
                Text = "Prev",
                Height = 40,
                Enabled = _page > 1
            };
            btnPrev.Click += async (s, e) =>
            {
                _page--;
                await LoadExamsAsync();
            };
            flowpnlBottom.Controls.Add(btnPrev);

            // Page numbers
            for (int i = 1; i <= _totalPages; i++)
            {
                var btn = new Button
                {
                    Text = i.ToString(),
                    Width = 40,
                    Height = 40,
                    BackColor = (i == _page) ? Color.LightSkyBlue : Color.White
                };
                int pageNum = i;

                btn.Click += async (s, e) =>
                {
                    _page = pageNum;
                    await LoadExamsAsync();
                };

                flowpnlBottom.Controls.Add(btn);
            }

            // Next
            var btnNext = new Button
            {
                Text = "Next",
                Height = 40,
                Enabled = _page < _totalPages
            };
            btnNext.Click += async (s, e) =>
            {
                _page++;
                await LoadExamsAsync();
            };
            flowpnlBottom.Controls.Add(btnNext);
        }
    }
}
