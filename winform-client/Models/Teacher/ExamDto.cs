using System;

namespace Academix.WinApp.Models.Teacher
{
    public class ExamDto
    {
        public int ExamId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        // Dùng double để map được cả giá trị nguyên và thập phân từ API
        public double TotalMarks { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsPublished { get; set; }
        public int QuestionCount { get; set; }
        public int AttemptCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


