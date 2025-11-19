using System.Collections.Generic;

namespace Academix.WinApp.Models.Teacher
{
    public class ExamListResponseDto
    {
        public List<ExamDto> Exams { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}


