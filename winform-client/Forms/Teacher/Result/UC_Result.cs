using Academix.WinApp.Api;
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
    public partial class UC_Result : UserControl
    {
        private int _classId = 0;
        private int _examId = 0;
        private int _page = 1;
        private int _totalPages = 1;
        private ExamApiService _examApi;
        private ClassApiService _classApi;

        public UC_Result()
        {
            InitializeComponent();
            _examApi = new ExamApiService();
            _classApi = new ClassApiService();
            
            // Wire up events
            this.Load += UC_Result_Load;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
            guna2Button3.Click += guna2Button3_Click;
            guna2Button4.Click += guna2Button4_Click;
            guna2Button5.Click += guna2Button5_Click;
            guna2Button6.Click += guna2Button6_Click;
            guna2Button7.Click += guna2Button7_Click;
        }

        private async void UC_Result_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            await LoadClassesAsync();
        }

        private void SetupDataGridView()
        {
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.Columns.Clear();
            guna2DataGridView1.CellFormatting += Guna2DataGridView1_CellFormatting;

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentName",
                HeaderText = "Họ tên",
                DataPropertyName = "StudentName",
                Width = 200
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentEmail",
                HeaderText = "Email",
                DataPropertyName = "StudentEmail",
                Width = 200
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalScore",
                HeaderText = "Điểm số",
                DataPropertyName = "TotalScore",
                Width = 100
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Percentage",
                HeaderText = "Phần trăm",
                DataPropertyName = "Percentage",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CorrectAnswers",
                HeaderText = "Câu đúng",
                DataPropertyName = "CorrectAnswers",
                Width = 100
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalQuestions",
                HeaderText = "Tổng câu",
                DataPropertyName = "TotalQuestions",
                Width = 100
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Trạng thái",
                DataPropertyName = "Status",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StartTime",
                HeaderText = "Bắt đầu",
                DataPropertyName = "StartTime",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });

            var submitTimeColumn = new DataGridViewTextBoxColumn
            {
                Name = "SubmitTime",
                HeaderText = "Nộp bài",
                DataPropertyName = "SubmitTime",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            };
            guna2DataGridView1.Columns.Add(submitTimeColumn);
        }

        private void Guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "SubmitTime" && e.Value == null)
            {
                e.Value = "Chưa nộp";
                e.FormattingApplied = true;
            }
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                await LoadExamsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadExamsAsync()
        {
            try
            {
                var myClasses = await _classApi.GetMyClassesAsync();

                if (myClasses == null || myClasses.Count == 0)
                {
                    MessageBox.Show("Bạn chưa có lớp học nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var examList = new List<ExamComboItem>();

                // Load exams from all classes
                foreach (var classItem in myClasses)
                {
                    try
                    {
                        var result = await _examApi.GetExamsByClassAsync(classItem.ClassId, page: 1, pageSize: 100);
                        if (result?.Data?.Exams != null)
                        {
                            foreach (var exam in result.Data.Exams)
                            {
                                examList.Add(new ExamComboItem
                                {
                                    ClassId = classItem.ClassId,
                                    ExamId = exam.ExamId,
                                    DisplayText = $"{classItem.ClassName} - {exam.Title}"
                                });
                            }
                        }
                    }
                    catch
                    {
                        // Continue loading other classes if one fails
                    }
                }

                if (examList.Count == 0)
                {
                    guna2ComboBox1.DataSource = null;
                    guna2ComboBox1.Items.Clear();
                    MessageBox.Show("Không có bài kiểm tra nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                guna2ComboBox1.DataSource = examList;
                guna2ComboBox1.DisplayMember = "DisplayText";
                guna2ComboBox1.ValueMember = "Key";

                guna2ComboBox1.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                guna2ComboBox1.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài kiểm tra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedValue != null && guna2ComboBox1.SelectedItem is ExamComboItem item)
            {
                _classId = item.ClassId;
                _examId = item.ExamId;
                _page = 1;
                await LoadResultsAsync();
            }
        }

        private class ExamComboItem
        {
            public int ClassId { get; set; }
            public int ExamId { get; set; }
            public string DisplayText { get; set; } = string.Empty;
            public string Key => $"{ClassId}_{ExamId}";
        }

        private async Task LoadResultsAsync()
        {
            try
            {
                if (_classId <= 0 || _examId <= 0)
                {
                    ClearResults();
                    return;
                }

                var result = await _examApi.GetExamResultsAsync(_classId, _examId, _page, 10);

                if (!result.Success || result.Data == null)
                {
                    MessageBox.Show(result.Message ?? "Không thể tải kết quả bài kiểm tra", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClearResults();
                    return;
                }

                var data = result.Data;

                // Update statistics
                UpdateStatistics(data.Statistics, data.TotalCount);

                // Update DataGridView
                guna2DataGridView1.DataSource = data.Results;

                // Update pagination
                _totalPages = data.TotalPages;
                UpdatePaginationUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải kết quả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatistics(ExamStatisticsDto stats, int totalCount)
        {
            lblSoLuongHocSinh.Text = totalCount.ToString();
            lblSLDaNop.Text = stats.CompletedAttempts.ToString();
            lblSLChuaNop.Text = (stats.TotalAttempts - stats.CompletedAttempts).ToString();
            lblDiemTrungBinh.Text = stats.AverageScore.ToString("F2");
        }

        private void ClearResults()
        {
            guna2DataGridView1.DataSource = null;
            lblSoLuongHocSinh.Text = "0";
            lblSLDaNop.Text = "0";
            lblSLChuaNop.Text = "0";
            lblDiemTrungBinh.Text = "0";
        }

        private void UpdatePaginationUI()
        {
            btnPrevious.Enabled = _page > 1;
            btnNext.Enabled = _page < _totalPages;

            // Update page number buttons
            guna2Button3.Text = "1";
            guna2Button3.Enabled = _page != 1;
            guna2Button4.Text = "2";
            guna2Button4.Enabled = _page != 2;
            guna2Button5.Text = "3";
            guna2Button5.Enabled = _page != 3;
            guna2Button6.Text = "4";
            guna2Button6.Enabled = _page != 4;
            guna2Button7.Text = "5";
            guna2Button7.Enabled = _page != 5;

            // Show/hide buttons based on total pages
            guna2Button3.Visible = _totalPages >= 1;
            guna2Button4.Visible = _totalPages >= 2;
            guna2Button5.Visible = _totalPages >= 3;
            guna2Button6.Visible = _totalPages >= 4;
            guna2Button7.Visible = _totalPages >= 5;
        }

        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                await LoadResultsAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_page < _totalPages)
            {
                _page++;
                await LoadResultsAsync();
            }
        }

        private async void guna2Button3_Click(object sender, EventArgs e)
        {
            _page = 1;
            await LoadResultsAsync();
        }

        private async void guna2Button4_Click(object sender, EventArgs e)
        {
            _page = 2;
            await LoadResultsAsync();
        }

        private async void guna2Button5_Click(object sender, EventArgs e)
        {
            _page = 3;
            await LoadResultsAsync();
        }

        private async void guna2Button6_Click(object sender, EventArgs e)
        {
            _page = 4;
            await LoadResultsAsync();
        }

        private async void guna2Button7_Click(object sender, EventArgs e)
        {
            _page = 5;
            await LoadResultsAsync();
        }
    }
}
