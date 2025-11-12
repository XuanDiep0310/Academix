using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Questions
{
    public class UpdateQuestionRequestDto
    {
        public string? QuestionText { get; set; }

        public string? QuestionType { get; set; }

        public string? DifficultyLevel { get; set; }

        [MaxLength(255)]
        public string? Subject { get; set; }

        public List<UpdateQuestionOptionDto>? Options { get; set; }
    }

    public class UpdateQuestionOptionDto
    {
        public int? OptionId { get; set; } // null for new options

        [Required(ErrorMessage = "Option text is required")]
        public string OptionText { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }

        [Required]
        public int OptionOrder { get; set; }
    }
}
