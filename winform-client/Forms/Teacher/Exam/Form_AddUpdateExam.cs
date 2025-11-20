using Academix.WinApp.Api;
using Academix.WinApp.Models.Teacher;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Teacher.Exam
{
    public partial class Form_AddUpdateExam : Form
    {
        private readonly int _classId;
        private readonly int _examId; // 0 = add
        private readonly ExamApiService _api;

        public event Func<Task>? OnSaved;

        public Form_AddUpdateExam(int classId, int examId = 0)
        {
            InitializeComponent();
            _classId = classId;
            _examId = examId;
            _api = new ExamApiService();
        }

        private async void Form_AddUpdateExam_Load(object sender, EventArgs e)
        {
            await LoadClassesAsync();
            if (_examId > 0)
            {
                Text = "Cập nhật bài kiểm tra";
                btnThem.Text = "Cập nhật";
                await LoadExamAsync();
            }
            else
            {
                Text = "Tạo bài kiểm tra mới";
            }

            txtMonHoc.Leave += async (s, ev) =>
            {
                await LoadCauHoiAsync();
            };
            dtpThoiGianBatDau.Value = DateTime.Now;
            dtpThoiGianKetThuc.Value = DateTime.Now.AddHours(1);

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

        private async Task LoadExamAsync()
        {
            var response = await _api.GetExamByIdAsync(_classId, _examId);

            if (!response.Success || response.Data == null)
            {
                MessageBox.Show("Không thể tải dữ liệu bài kiểm tra!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var exam = response.Data;

            txtTieuDe.Text = exam.Title;
            nmbThoiLuongLamBai.Value = exam.Duration;
            dtpThoiGianBatDau.Value = exam.StartTime?.ToLocalTime() ?? DateTime.Now;
            dtpThoiGianKetThuc.Value = exam.EndTime?.ToLocalTime() ?? DateTime.Now.AddHours(1);
            ckbCauHoi.Checked = exam.IsPublished;

            await LoadCauHoiAsync();

            // Check sẵn câu hỏi đã có trong exam
            var examQuestionsResp = await _api.GetExamQuestionsAsync(_classId, _examId);
            if (examQuestionsResp.Success && examQuestionsResp.Data != null)
            {
                var selectedIds = examQuestionsResp.Data.Select(q => q.QuestionId).ToList();
                foreach (Control ctrl in pnlCauHoi.Controls)
                {
                    if (ctrl is Guna2CheckBox chk && selectedIds.Contains((int)chk.Tag))
                        chk.Checked = true;
                }
            }
        }

        private async Task LoadCauHoiAsync()
        {
            pnlCauHoi.Controls.Clear();

            // Lọc danh sách câu hỏi theo môn học (nếu có)
            string? subject = string.IsNullOrWhiteSpace(txtMonHoc.Text)
                ? null
                : txtMonHoc.Text.Trim();

            var questionApi = new QuestionApiService();
            var questionsResponse = await questionApi.GetMyQuestionsPagedAsync(
                subject: subject,
                page: 1,
                pageSize: 100);

            int y = 5;
            foreach (var q in questionsResponse.Data?.Questions ?? new List<QuestionResponseDto>())
            {
                var chk = new Guna2CheckBox
                {
                    Text = q.QuestionText,
                    Tag = q.QuestionId,
                    Checked = false,
                    Location = new Point(5, y),
                    AutoSize = true
                };

                chk.CheckedChanged += (s, e) => UpdateSoLuongCauHoi();

                pnlCauHoi.Controls.Add(chk);
                y += 25;
            }

            // Nếu đang update, check sẵn các câu hỏi đã có trong exam
            if (_examId > 0)
            {
                var examQuestionsResp = await _api.GetExamQuestionsAsync(_classId, _examId);
                if (examQuestionsResp.Success && examQuestionsResp.Data != null)
                {
                    var selectedIds = examQuestionsResp.Data.Select(q => q.QuestionId).ToList();
                    foreach (Control ctrl in pnlCauHoi.Controls)
                    {
                        if (ctrl is Guna2CheckBox chk && selectedIds.Contains((int)chk.Tag))
                            chk.Checked = true;
                    }
                }
            }
            UpdateSoLuongCauHoi();
        }


        private async void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ!", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_examId == 0)
                await CreateExamAsync();
            else
                await UpdateExamAsync();
        }

        private async Task CreateExamAsync()
        {
            // Lấy danh sách câu hỏi được chọn
            var selectedQuestions = pnlCauHoi.Controls.OfType<Guna2CheckBox>()
                .Where(c => c.Checked)
                .Select((c, index) => new ExamQuestionDto
                {
                    QuestionId = (int)c.Tag,
                    QuestionOrder = index + 1,
                    Marks = 1
                })
                .ToList();

            var request = new CreateExamRequestDto
            {
                Title = txtTieuDe.Text.Trim(),
                Description = lblMoTa.Text.Trim(),
                Duration = (int)nmbThoiLuongLamBai.Value,
                TotalMarks = selectedQuestions.Sum(q => q.Marks),
                StartTime = dtpThoiGianBatDau.Value,
                EndTime = dtpThoiGianKetThuc.Value,
                Questions = selectedQuestions 
            };

            var result = await _api.CreateExamAsync(_classId, request);

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Tạo bài kiểm tra thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (OnSaved != null) await OnSaved();
            Close();
        }


        private async Task UpdateExamAsync()
        {
            // Lấy danh sách câu hỏi mới được chọn
            var selectedQuestions = pnlCauHoi.Controls.OfType<Guna2CheckBox>()
                .Where(c => c.Checked)
                .Select((c, index) => new ExamQuestionDto
                {
                    QuestionId = (int)c.Tag,
                    QuestionOrder = index + 1,
                    Marks = 1
                })
                .ToList();

            var request = new UpdateExamRequestDto
            {
                Title = txtTieuDe.Text.Trim(),
                Description = txtMoTa.Text.Trim(),
                Duration = (int)nmbThoiLuongLamBai.Value,
                TotalMarks = selectedQuestions.Sum(q => q.Marks),
                StartTime = dtpThoiGianBatDau.Value,
                EndTime = dtpThoiGianKetThuc.Value
            };

            var result = await _api.UpdateExamAsync(_classId, _examId, request);

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Xoá câu hỏi cũ
            var existing = await _api.GetExamQuestionsAsync(_classId, _examId);
            if (existing.Success && existing.Data != null)
            {
                foreach (var q in existing.Data)
                {
                    await _api.RemoveQuestionFromExamAsync(_classId, _examId, q.QuestionId);
                }
            }

            // Thêm câu hỏi mới
            if (selectedQuestions.Count > 0)
            {
                var addReq = new AddQuestionsToExamRequestDto
                {
                    Questions = selectedQuestions
                };

                await _api.AddQuestionsToExamAsync(_classId, _examId, addReq);
            }

            MessageBox.Show("Cập nhật bài kiểm tra thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (OnSaved != null) await OnSaved();
            Close();
        }
        private void UpdateSoLuongCauHoi()
        {
            int count = pnlCauHoi.Controls.OfType<Guna2CheckBox>().Count(c => c.Checked);
            lblSoLuongCauHoi.Text = $"Đã chọn: {count} câu hỏi";
        }



        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtTieuDe.Text)) { txtTieuDe.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtMonHoc.Text)) { txtMonHoc.Focus(); return false; }
            if (nmbThoiLuongLamBai.Value <= 0) { nmbThoiLuongLamBai.Focus(); return false; }
            if (dtpThoiGianKetThuc.Value <= dtpThoiGianBatDau.Value) { dtpThoiGianKetThuc.Focus(); return false; }
            if (!pnlCauHoi.Controls.OfType<Guna2CheckBox>().Any(c => c.Checked)) { pnlCauHoi.Focus(); return false; }
            return true;
        }
    }
}
