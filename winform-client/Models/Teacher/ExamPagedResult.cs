using System.Collections.Generic;

namespace Academix.WinApp.Models.Teacher
{
    public class ExamPagedResult
    {
        public List<ExamDto> Exams { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}


