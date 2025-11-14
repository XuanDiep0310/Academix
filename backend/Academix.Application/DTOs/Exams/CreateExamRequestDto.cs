using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Exams
{
    public class CreateExamRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes")]
        public int Duration { get; set; }

        [Range(0.01, 1000, ErrorMessage = "Total marks must be between 0.01 and 1000")]
        public decimal TotalMarks { get; set; } = 100;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<ExamQuestionDto> Questions { get; set; } = new();
    }

    public class ExamQuestionDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Question order must be between 1 and 1000")]
        public int QuestionOrder { get; set; }

        [Range(0.01, 100, ErrorMessage = "Marks must be between 0.01 and 100")]
        public decimal Marks { get; set; } = 1;
    }
}
