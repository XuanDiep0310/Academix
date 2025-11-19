using System;
using System.Collections.Generic;

namespace Academix.WinApp.Models.Student
{
    public class ExamOptionDto
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OptionOrder { get; set; }
    }

    public class ExamQuestionDto
    {
        public int ExamQuestionId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int QuestionOrder { get; set; }
        public double Marks { get; set; }
        public List<ExamOptionDto> Options { get; set; } = new();
    }

    public class StartExamResponseDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<ExamQuestionDto> Questions { get; set; } = new();
    }

    public class ExamAnswerRequestDto
    {
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
    }
}


