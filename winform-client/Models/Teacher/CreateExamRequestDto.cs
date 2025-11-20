using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class CreateExamRequestDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Duration { get; set; }

        public decimal TotalMarks { get; set; } = 100;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public List<ExamQuestionDto> Questions { get; set; } = new();
    }

    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }

        public int QuestionOrder { get; set; }

        public decimal Marks { get; set; } = 1;
    }

}
