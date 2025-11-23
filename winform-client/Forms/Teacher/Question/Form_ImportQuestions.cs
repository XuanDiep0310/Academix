using Academix.WinApp.Api;
using Academix.WinApp.Models.Teacher;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher.Question
{
    public partial class Form_ImportQuestions : Form
    {
        private readonly QuestionApiService _api = new QuestionApiService();
        private DataTable _importedData;
        
        private Guna.UI2.WinForms.Guna2Panel topPanel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2Button btnSelectFile;
        private Guna.UI2.WinForms.Guna2Button btnDownloadTemplate;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblFileName;
        private Guna.UI2.WinForms.Guna2DataGridView dgvPreview;
        private Guna.UI2.WinForms.Guna2Panel bottomPanel;
        private Guna.UI2.WinForms.Guna2Button btnImport;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Panel mainPanel;

        public Form_ImportQuestions()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            topPanel = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnSelectFile = new Guna.UI2.WinForms.Guna2Button();
            btnDownloadTemplate = new Guna.UI2.WinForms.Guna2Button();
            lblFileName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            dgvPreview = new Guna.UI2.WinForms.Guna2DataGridView();
            bottomPanel = new Guna.UI2.WinForms.Guna2Panel();
            btnImport = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            mainPanel = new Panel();
            
            topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).BeginInit();
            bottomPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            SuspendLayout();
            
            // topPanel
            topPanel.BackColor = Color.White;
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(btnSelectFile);
            topPanel.Controls.Add(btnDownloadTemplate);
            topPanel.Controls.Add(lblFileName);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(20);
            topPanel.Size = new Size(1000, 120);
            topPanel.TabIndex = 0;
            
            // lblTitle
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(70, 130, 180);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(500, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📥 Import câu hỏi từ Excel";
            
            // btnSelectFile
            btnSelectFile.BorderRadius = 15;
            btnSelectFile.FillColor = Color.FromArgb(70, 130, 180);
            btnSelectFile.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSelectFile.ForeColor = Color.White;
            btnSelectFile.Location = new Point(20, 60);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(180, 40);
            btnSelectFile.TabIndex = 1;
            btnSelectFile.Text = "📁 Chọn file Excel";
            btnSelectFile.Click += BtnSelectFile_Click;
            
            // btnDownloadTemplate
            btnDownloadTemplate.BorderRadius = 15;
            btnDownloadTemplate.FillColor = Color.FromArgb(108, 117, 125);
            btnDownloadTemplate.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnDownloadTemplate.ForeColor = Color.White;
            btnDownloadTemplate.Location = new Point(220, 60);
            btnDownloadTemplate.Name = "btnDownloadTemplate";
            btnDownloadTemplate.Size = new Size(200, 40);
            btnDownloadTemplate.TabIndex = 2;
            btnDownloadTemplate.Text = "📄 Tải mẫu Excel";
            btnDownloadTemplate.Click += BtnDownloadTemplate_Click;
            
            // lblFileName
            lblFileName.BackColor = Color.Transparent;
            lblFileName.Font = new Font("Segoe UI", 11F);
            lblFileName.ForeColor = Color.FromArgb(80, 80, 80);
            lblFileName.Location = new Point(440, 70);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(540, 20);
            lblFileName.TabIndex = 3;
            lblFileName.Text = "Chưa chọn file";
            
            // dgvPreview
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.AllowUserToDeleteRows = false;
            dgvPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPreview.BackgroundColor = Color.White;
            dgvPreview.ColumnHeadersHeight = 40;
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.GridColor = Color.FromArgb(231, 229, 255);
            dgvPreview.Location = new Point(0, 0);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.ReadOnly = true;
            dgvPreview.RowHeadersVisible = false;
            dgvPreview.Size = new Size(1000, 350);
            dgvPreview.TabIndex = 1;
            dgvPreview.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.FeterRiver;
            dgvPreview.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(70, 130, 180);
            dgvPreview.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgvPreview.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            
            // bottomPanel
            bottomPanel.BackColor = Color.White;
            bottomPanel.Controls.Add(lblStatus);
            bottomPanel.Controls.Add(btnImport);
            bottomPanel.Controls.Add(btnCancel);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 350);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Padding = new Padding(20);
            bottomPanel.Size = new Size(1000, 100);
            bottomPanel.TabIndex = 2;
            
            // btnImport
            btnImport.BorderRadius = 15;
            btnImport.Enabled = false;
            btnImport.FillColor = Color.FromArgb(40, 167, 69);
            btnImport.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnImport.ForeColor = Color.White;
            btnImport.Location = new Point(700, 20);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(130, 45);
            btnImport.TabIndex = 0;
            btnImport.Text = "✅ Import";
            btnImport.Click += BtnImport_Click;
            
            // btnCancel
            btnCancel.BorderRadius = 15;
            btnCancel.FillColor = Color.FromArgb(220, 53, 69);
            btnCancel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(850, 20);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(130, 45);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "❌ Hủy";
            btnCancel.Click += BtnCancel_Click;
            
            // lblStatus
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 11F);
            lblStatus.ForeColor = Color.FromArgb(50, 50, 50);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(20, 30);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(650, 20);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Vui lòng chọn file Excel để xem trước";
            
            // mainPanel
            mainPanel.Controls.Add(dgvPreview);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 120);
            mainPanel.Name = "mainPanel";
            mainPanel.Padding = new Padding(20);
            mainPanel.Size = new Size(1000, 350);
            mainPanel.TabIndex = 1;
            
            // Form
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 450);
            Controls.Add(mainPanel);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
            MinimumSize = new Size(800, 500);
            Name = "Form_ImportQuestions";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import câu hỏi từ Excel";
            
            topPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPreview).EndInit();
            bottomPanel.ResumeLayout(false);
            mainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Chọn file Excel chứa câu hỏi"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadExcelFile(openDialog.FileName);
                    lblFileName.Text = Path.GetFileName(openDialog.FileName);
                    btnImport.Enabled = true;
                    lblStatus.Text = $"Đã tải {_importedData?.Rows.Count ?? 0} câu hỏi từ file";
                    lblStatus.ForeColor = Color.FromArgb(40, 167, 69);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Lỗi khi đọc file";
                    lblStatus.ForeColor = Color.FromArgb(220, 53, 69);
                    btnImport.Enabled = false;
                }
            }
        }

        private void LoadExcelFile(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

            _importedData = new DataTable();
            
            // Đọc header
            var headerRow = worksheet.Row(1);
            var headers = new List<string>();
            int col = 1;
            while (true)
            {
                var cellValue = worksheet.Cell(1, col).Value.ToString();
                if (string.IsNullOrWhiteSpace(cellValue)) break;
                headers.Add(cellValue.Trim());
                _importedData.Columns.Add(cellValue.Trim());
                col++;
            }

            // Đọc dữ liệu
            int row = 2;
            while (true)
            {
                var firstCell = worksheet.Cell(row, 1).Value.ToString();
                if (string.IsNullOrWhiteSpace(firstCell)) break;

                var dataRow = _importedData.NewRow();
                for (int c = 0; c < headers.Count; c++)
                {
                    dataRow[c] = worksheet.Cell(row, c + 1).Value.ToString();
                }
                _importedData.Rows.Add(dataRow);
                row++;
            }

            dgvPreview.DataSource = _importedData;
        }

        private void BtnDownloadTemplate_Click(object sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = "Mau_CauHoi.xlsx",
                Title = "Lưu file mẫu Excel"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CreateTemplateFile(saveDialog.FileName);
                    MessageBox.Show("Đã tạo file mẫu thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Mở file vừa tạo
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tạo file mẫu: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CreateTemplateFile(string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Câu hỏi");

            // Header
            worksheet.Cell(1, 1).Value = "Câu hỏi";
            worksheet.Cell(1, 2).Value = "Môn học";
            worksheet.Cell(1, 3).Value = "Độ khó";
            worksheet.Cell(1, 4).Value = "Đáp án 1";
            worksheet.Cell(1, 5).Value = "Đáp án 2";
            worksheet.Cell(1, 6).Value = "Đáp án 3";
            worksheet.Cell(1, 7).Value = "Đáp án 4";
            worksheet.Cell(1, 8).Value = "Đáp án đúng";

            // Style header
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(70, 130, 180);
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Dữ liệu mẫu
            worksheet.Cell(2, 1).Value = "Câu hỏi mẫu: 2 + 2 = ?";
            worksheet.Cell(2, 2).Value = "Toán học";
            worksheet.Cell(2, 3).Value = "Easy";
            worksheet.Cell(2, 4).Value = "3";
            worksheet.Cell(2, 5).Value = "4";
            worksheet.Cell(2, 6).Value = "5";
            worksheet.Cell(2, 7).Value = "6";
            worksheet.Cell(2, 8).Value = "2"; // Đáp án đúng là đáp án số 2 (đáp án 2)

            // Ghi chú
            worksheet.Cell(4, 1).Value = "Ghi chú:";
            worksheet.Cell(5, 1).Value = "- Độ khó: Easy, Medium, Hard";
            worksheet.Cell(6, 1).Value = "- Đáp án đúng: Nhập số thứ tự đáp án (1, 2, 3, hoặc 4)";
            worksheet.Cell(7, 1).Value = "- Có thể có ít nhất 2 đáp án, tối đa 4 đáp án";

            // Auto fit columns
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
        }

        private async void BtnImport_Click(object sender, EventArgs e)
        {
            if (_importedData == null || _importedData.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để import!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnImport.Enabled = false;
            lblStatus.Text = "Đang import câu hỏi...";
            lblStatus.ForeColor = Color.FromArgb(70, 130, 180);

            try
            {
                int successCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                foreach (DataRow row in _importedData.Rows)
                {
                    try
                    {
                        var question = ParseQuestionRow(row);
                        if (question == null)
                        {
                            errorCount++;
                            errors.Add($"Dòng {_importedData.Rows.IndexOf(row) + 2}: Dữ liệu không hợp lệ");
                            continue;
                        }

                        var result = await _api.CreateQuestionAsync(question);
                        if (result.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorCount++;
                            errors.Add($"Dòng {_importedData.Rows.IndexOf(row) + 2}: {result.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Dòng {_importedData.Rows.IndexOf(row) + 2}: {ex.Message}");
                    }
                }

                string message = $"Import hoàn tất!\nThành công: {successCount}\nLỗi: {errorCount}";
                if (errors.Any())
                {
                    message += $"\n\nChi tiết lỗi:\n{string.Join("\n", errors.Take(10))}";
                    if (errors.Count > 10)
                        message += $"\n... và {errors.Count - 10} lỗi khác";
                }

                MessageBox.Show(message, "Kết quả import",
                    MessageBoxButtons.OK,
                    errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                if (successCount > 0)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi import: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnImport.Enabled = true;
                lblStatus.Text = "Import hoàn tất";
            }
        }

        private CreateQuestionRequestDto? ParseQuestionRow(DataRow row)
        {
            try
            {
                // Lấy giá trị từ các cột
                string questionText = GetCellValue(row, "Câu hỏi");
                string subject = GetCellValue(row, "Môn học");
                string difficulty = GetCellValue(row, "Độ khó");
                string option1 = GetCellValue(row, "Đáp án 1");
                string option2 = GetCellValue(row, "Đáp án 2");
                string option3 = GetCellValue(row, "Đáp án 3");
                string option4 = GetCellValue(row, "Đáp án 4");
                string correctAnswerStr = GetCellValue(row, "Đáp án đúng");

                // Validate
                if (string.IsNullOrWhiteSpace(questionText) || 
                    string.IsNullOrWhiteSpace(subject) ||
                    string.IsNullOrWhiteSpace(difficulty))
                {
                    return null;
                }

                // Validate độ khó
                difficulty = difficulty.Trim();
                if (difficulty != "Easy" && difficulty != "Medium" && difficulty != "Hard")
                {
                    return null;
                }

                // Lấy đáp án
                var options = new List<CreateQuestionOptionDto>();
                int order = 1;
                if (!string.IsNullOrWhiteSpace(option1))
                {
                    options.Add(new CreateQuestionOptionDto
                    {
                        OptionText = option1.Trim(),
                        OptionOrder = order++,
                        IsCorrect = false
                    });
                }
                if (!string.IsNullOrWhiteSpace(option2))
                {
                    options.Add(new CreateQuestionOptionDto
                    {
                        OptionText = option2.Trim(),
                        OptionOrder = order++,
                        IsCorrect = false
                    });
                }
                if (!string.IsNullOrWhiteSpace(option3))
                {
                    options.Add(new CreateQuestionOptionDto
                    {
                        OptionText = option3.Trim(),
                        OptionOrder = order++,
                        IsCorrect = false
                    });
                }
                if (!string.IsNullOrWhiteSpace(option4))
                {
                    options.Add(new CreateQuestionOptionDto
                    {
                        OptionText = option4.Trim(),
                        OptionOrder = order++,
                        IsCorrect = false
                    });
                }

                if (options.Count < 2)
                {
                    return null;
                }

                // Parse đáp án đúng
                if (!int.TryParse(correctAnswerStr, out int correctIndex) || 
                    correctIndex < 1 || correctIndex > options.Count)
                {
                    return null;
                }

                // Đánh dấu đáp án đúng (index từ 1)
                options[correctIndex - 1].IsCorrect = true;

                return new CreateQuestionRequestDto
                {
                    QuestionText = questionText.Trim(),
                    Subject = subject.Trim(),
                    DifficultyLevel = difficulty,
                    QuestionType = "SingleChoice",
                    Options = options
                };
            }
            catch
            {
                return null;
            }
        }

        private string GetCellValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return row[columnName]?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

