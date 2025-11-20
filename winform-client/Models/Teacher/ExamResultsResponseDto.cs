using System.Collections.Generic;

namespace Academix.WinApp.Models.Teacher
{
    public class ExamResultsResponseDto
    {
        public List<ExamResultItemDto> Results { get; set; } = new List<ExamResultItemDto>();
        public ExamStatisticsDto Statistics { get; set; } = new ExamStatisticsDto();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

