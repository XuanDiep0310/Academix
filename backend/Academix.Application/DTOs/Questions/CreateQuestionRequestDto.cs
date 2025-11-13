using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Questions
{
    public class CreateQuestionRequestDto
    {
        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Question type is required")]
        public string QuestionType { get; set; } = "MultipleChoice";

        public string? DifficultyLevel { get; set; }

        [MaxLength(255)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "At least one option is required")]
        [MinLength(2, ErrorMessage = "At least 2 options are required")]
        public List<CreateQuestionOptionDto> Options { get; set; } = new();
    }

    public class CreateQuestionOptionDto
    {
        [Required(ErrorMessage = "Option text is required")]
        public string OptionText { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }

        [Required]
        public int OptionOrder { get; set; }
    }
}
