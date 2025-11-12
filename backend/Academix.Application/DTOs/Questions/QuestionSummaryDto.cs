using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Questions
{
    public class QuestionSummaryDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string? DifficultyLevel { get; set; }
        public string? Subject { get; set; }
        public int OptionsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
