using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Questions
{
    public class BulkCreateQuestionsResponseDto
    {
        public List<QuestionResponseDto> SuccessfulQuestions { get; set; } = new();
        public List<QuestionCreationFailedDto> FailedQuestions { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class QuestionCreationFailedDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int Index { get; set; }
    }
}
