using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Exams
{
    public class ExamResultsListResponseDto
    {
        public List<ExamResultSummaryDto> Results { get; set; } = new();
        public ExamStatisticsSummaryDto Statistics { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ExamResultSummaryDto
    {
        public int AttemptId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public decimal? TotalScore { get; set; }
        public decimal? Percentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class ExamStatisticsSummaryDto
    {
        public decimal? AverageScore { get; set; }
        public decimal? HighestScore { get; set; }
        public decimal? LowestScore { get; set; }
        public int TotalAttempts { get; set; }
        public int CompletedAttempts { get; set; }
        public int InProgressAttempts { get; set; }
        public decimal? PassRate { get; set; }
    }
}
