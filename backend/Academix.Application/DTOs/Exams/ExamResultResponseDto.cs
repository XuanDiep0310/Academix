using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Exams
{
    public class ExamResultResponseDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public decimal? TotalScore { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal? Percentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public List<StudentAnswerDetailDto> Answers { get; set; } = new();
    }

    public class StudentAnswerDetailDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int? SelectedOptionId { get; set; }
        public string? SelectedOptionText { get; set; }
        public int? CorrectOptionId { get; set; }
        public string? CorrectOptionText { get; set; }
        public bool? IsCorrect { get; set; }
        public decimal? MarksObtained { get; set; }
        public decimal TotalMarks { get; set; }
    }
}
