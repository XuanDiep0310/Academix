using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class UpdateQuestionRequestDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? DifficultyLevel { get; set; }
        public List<CreateQuestionOptionDto> Options { get; set; } = new();
    }

}
