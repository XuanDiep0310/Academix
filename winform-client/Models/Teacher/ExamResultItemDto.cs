using System;

namespace Academix.WinApp.Models.Teacher
{
    public class ExamResultItemDto
    {
        public int AttemptId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public double TotalScore { get; set; }
        public double Percentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
    }
}

