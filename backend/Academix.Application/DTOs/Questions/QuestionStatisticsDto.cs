using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Questions
{
    public class QuestionStatisticsDto
    {
        public int TotalQuestions { get; set; }
        public Dictionary<string, int> QuestionsByType { get; set; } = new();
        public Dictionary<string, int> QuestionsByDifficulty { get; set; } = new();
        public Dictionary<string, int> QuestionsBySubject { get; set; } = new();
        public int QuestionsCreatedToday { get; set; }
        public int QuestionsCreatedThisWeek { get; set; }
        public int QuestionsCreatedThisMonth { get; set; }
        public List<TopQuestionCreatorDto> TopCreators { get; set; } = new();
    }

    public class TopQuestionCreatorDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }
}
