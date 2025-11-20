using Academix.WinApp.Api;
using Academix.WinApp.Models.Teacher;
using ClosedXML.Excel;
using System.Diagnostics;
using DocumentFormat.OpenXml.VariantTypes;
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
        private ExamApiService _examApi = new ExamApiService();
        private ClassApiService _classApi = new ClassApiService();

        public UC_Result()
        {
            InitializeComponent();

            // Wire up events
            this.Load += UC_Result_Load;
        }

        private async void UC_Result_Load(object sender, EventArgs e)
        {
            ClearResults();
            SetupDataGridView();
            await LoadClassesAsync();
            await LoadExamsAsync();
        }

        private void SetupDataGridView()
        {
            dgvHocSinh.AutoGenerateColumns = false;
            dgvHocSinh.Columns.Clear();
            dgvHocSinh.CellFormatting += Guna2DataGridView1_CellFormatting;

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentName",
                HeaderText = "Họ tên",
                DataPropertyName = "StudentName",
                Width = 200
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentEmail",
                HeaderText = "Email",
                DataPropertyName = "StudentEmail",
                Width = 200
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalScore",
                HeaderText = "Điểm số",
                DataPropertyName = "TotalScore",
                Width = 100
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Percentage",
                HeaderText = "Phần trăm",
                DataPropertyName = "Percentage",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CorrectAnswers",
                HeaderText = "Câu đúng",
                DataPropertyName = "CorrectAnswers",
                Width = 100
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalQuestions",
                HeaderText = "Tổng câu",
                DataPropertyName = "TotalQuestions",
                Width = 100
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Trạng thái",
                DataPropertyName = "Status",
                Width = 120
            });

            dgvHocSinh.Columns.Add(new DataGridViewTextBoxColumn
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
            dgvHocSinh.Columns.Add(submitTimeColumn);
        }

        private void Guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvHocSinh.Columns[e.ColumnIndex].Name == "SubmitTime" && e.Value == null)
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
                        var result = await _examApi.GetExamsByClassAsync(classItem.ClassId, isPublished: true);
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
                    cmbBaiKT.DataSource = null;
                    cmbBaiKT.Items.Clear();
                    MessageBox.Show("Không có bài kiểm tra nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                cmbBaiKT.DataSource = examList;
                cmbBaiKT.DisplayMember = "DisplayText";
                cmbBaiKT.ValueMember = "Key";

                cmbBaiKT.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                cmbBaiKT.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài kiểm tra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBaiKT.SelectedValue != null && cmbBaiKT.SelectedItem is ExamComboItem item)
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

                // Lấy tổng số học viên của lớp
                var classInfo = await _classApi.GetClassByIdAsync(_classId);
                lblSoLuongHocSinh.Text = classInfo?.StudentCount.ToString() ?? "0";

                // Lấy kết quả bài kiểm tra
                var result = await _examApi.GetExamResultsAsync(_classId, _examId);

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
                dgvHocSinh.DataSource = data.Results;

                // Update pagination
                _totalPages = data.TotalPages;

                BuildPaginationUI();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải kết quả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatistics(ExamStatisticsDto stats, int totalCount)
        {
            lblSLDaNop.Text = stats.CompletedAttempts.ToString();
            lblSLChuaNop.Text = (stats.TotalAttempts - stats.CompletedAttempts).ToString();
            lblDiemTrungBinh.Text = stats.AverageScore?.ToString("F2") ?? "0";
        }

        private void ClearResults()
        {
            dgvHocSinh.DataSource = null;
            lblSoLuongHocSinh.Text = "0";
            lblSLDaNop.Text = "0";
            lblSLChuaNop.Text = "0";
            lblDiemTrungBinh.Text = "0";
        }

        private void BuildPaginationUI()
        {
            flowpnlPagination.Controls.Clear();

            // Prev button
            var btnPrev = new Button
            {
                Text = "Prev",
                Height = 40,
                Width = 50,
                Enabled = _page > 1
            };
            btnPrev.Click += async (s, e) =>
            {
                if (_page > 1)
                {
                    _page--;
                    await LoadResultsAsync();
                }
            };
            flowpnlPagination.Controls.Add(btnPrev);

            // Page buttons (hiển thị tối đa 5 page như trước)
            int maxPagesToShow = 5;
            int start = Math.Max(1, _page - 2);
            int end = Math.Min(_totalPages, start + maxPagesToShow - 1);

            for (int i = start; i <= end; i++)
            {
                var btnPage = new Button
                {
                    Text = i.ToString(),
                    Width = 40,
                    Height = 40,
                    BackColor = (i == _page) ? Color.LightSkyBlue : Color.White
                };
                int pageNum = i; // tránh closure
                btnPage.Click += async (s, e) =>
                {
                    _page = pageNum;
                    await LoadResultsAsync();
                };
                flowpnlPagination.Controls.Add(btnPage);
            }

            // Next button
            var btnNext = new Button
            {
                Text = "Next",
                Height = 40,
                Width = 50,
                Enabled = _page < _totalPages
            };
            btnNext.Click += async (s, e) =>
            {
                if (_page < _totalPages)
                {
                    _page++;
                    await LoadResultsAsync();
                }
            };
            flowpnlPagination.Controls.Add(btnNext);
        }





        private void btnXuatExcel_Click(object sender, EventArgs e)
    {
        if (_examId <= 0 || _classId <= 0)
        {
            MessageBox.Show("Vui lòng chọn bài kiểm tra!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvHocSinh.Rows.Count == 0)
        {
            MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using (SaveFileDialog saveDialog = new SaveFileDialog())
        {
            saveDialog.Filter = "Excel File|*.xlsx";
            saveDialog.Title = "Xuất bảng điểm";
            saveDialog.FileName = $"BangDiem_Exam_{_examId}.xlsx";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("BangDiem");

                        // Header
                        for (int i = 0; i < dgvHocSinh.Columns.Count; i++)
                        {
                            ws.Cell(1, i + 1).Value = dgvHocSinh.Columns[i].HeaderText;
                            ws.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        // Data
                        for (int i = 0; i < dgvHocSinh.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgvHocSinh.Columns.Count; j++)
                            {
                                ws.Cell(i + 2, j + 1).Value =
                                    dgvHocSinh.Rows[i].Cells[j].Value?.ToString();
                            }
                        }

                        ws.Columns().AdjustToContents();

                        // Save file
                        workbook.SaveAs(saveDialog.FileName);
                    }

                    MessageBox.Show("Xuất Excel thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở file Excel vừa xuất
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true // dùng shell để mở bằng ứng dụng mặc định
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }


}
}
