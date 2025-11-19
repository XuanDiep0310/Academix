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
            dtpThoiGianBatDau.Value = exam.StartTime ?? DateTime.Now;
            dtpThoiGianKetThuc.Value = exam.EndTime ?? DateTime.Now.AddHours(1);
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

            // Load tất cả câu hỏi 
            var questions = await _api.GetExamQuestionsAsync(_classId, _examId);

            int y = 5;
            foreach (var q in questions.Data ?? new List<ExamQuestionDetailDto>())
            {
                var chk = new Guna2CheckBox
                {
                    Text = q.QuestionOrder.ToString(), 
                    Tag = q.QuestionId,
                    Checked = false,
                    Location = new Point(5, y),
                    AutoSize = true
                };
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
            var request = new CreateExamRequestDto
            {
                Title = txtTieuDe.Text.Trim(),
                Duration = (int)nmbThoiLuongLamBai.Value,
                StartTime = dtpThoiGianBatDau.Value,
                EndTime = dtpThoiGianKetThuc.Value
            };

            var result = await _api.CreateExamAsync(_classId, request);

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int newExamId = result.Data!.ExamId;

            // Thêm câu hỏi đã chọn
            var selectedQuestions = pnlCauHoi.Controls.OfType<Guna2CheckBox>()
                .Where(c => c.Checked)
                .Select(c => new ExamQuestionDto { QuestionId = (int)c.Tag, Marks = 1 })
                .ToList();

            if (selectedQuestions.Count > 0)
            {
                var addReq = new AddQuestionsToExamRequestDto { Questions = selectedQuestions };
                await _api.AddQuestionsToExamAsync(_classId, newExamId, addReq);
            }

            MessageBox.Show("Tạo bài kiểm tra thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (OnSaved != null) await OnSaved();
            Close();
        }

        private async Task UpdateExamAsync()
        {
            var request = new UpdateExamRequestDto
            {
                Title = txtTieuDe.Text.Trim(),
                Duration = (int)nmbThoiLuongLamBai.Value,
                StartTime = dtpThoiGianBatDau.Value,
                EndTime = dtpThoiGianKetThuc.Value
            };

            var result = await _api.UpdateExamAsync(_classId, _examId, request);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Xoá câu hỏi cũ, thêm câu hỏi mới
            var existingQuestionsResp = await _api.GetExamQuestionsAsync(_classId, _examId);
            if (existingQuestionsResp.Success && existingQuestionsResp.Data != null)
            {
                var oldIds = existingQuestionsResp.Data.Select(q => q.QuestionId).ToList();
                foreach (var qid in oldIds)
                {
                    await _api.RemoveQuestionFromExamAsync(_classId, _examId, qid);
                }
            }

            var selectedQuestions = pnlCauHoi.Controls.OfType<Guna2CheckBox>()
                .Where(c => c.Checked)
                .Select(c => new ExamQuestionDto { QuestionId = (int)c.Tag, Marks = 1 })
                .ToList();

            if (selectedQuestions.Count > 0)
            {
                var addReq = new AddQuestionsToExamRequestDto { Questions = selectedQuestions };
                await _api.AddQuestionsToExamAsync(_classId, _examId, addReq);
            }

            MessageBox.Show("Cập nhật bài kiểm tra thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (OnSaved != null) await OnSaved();
            Close();
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
