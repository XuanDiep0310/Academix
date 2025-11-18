using System;
using System.Collections.Generic;

namespace Academix.WinApp.Models.Student
{
    public class StudentExamAnswerDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int SelectedOptionId { get; set; }
        public string? SelectedOptionText { get; set; }
        public int CorrectOptionId { get; set; }
        public string CorrectOptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public double MarksObtained { get; set; }
        public double TotalMarks { get; set; }
    }

    public class StudentExamResultDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public double TotalScore { get; set; }
        public double TotalMarks { get; set; }
        public double? Percentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public List<StudentExamAnswerDto> Answers { get; set; } = new();
    }
}


