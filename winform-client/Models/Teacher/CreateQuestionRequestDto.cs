using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class CreateQuestionRequestDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? DifficultyLevel { get; set; }
        public string QuestionType { get; set; } = "MultipleChoice";
        public List<CreateQuestionOptionDto> Options { get; set; } = new();
    }

    public class CreateQuestionOptionDto
    {
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OptionOrder { get; set; }
    }


}
