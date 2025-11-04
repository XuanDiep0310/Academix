using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Teacher
{
    public class TeacherDashboardDto
    {
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveExams { get; set; }
        public int PendingGrading { get; set; }

        public List<HighRiskStudentDto> HighRiskStudents { get; set; } = new();
        public List<RecentExamDto> RecentExams { get; set; } = new();
        public List<ClassStatDto> ClassStats { get; set; } = new();
    }

    public class HighRiskStudentDto
    {
        public string StudentName { get; set; } = "";
        public string ExamTitle { get; set; } = "";
        public int FocusLossCount { get; set; }
        public int CopyPasteCount { get; set; }
    }

    public class RecentExamDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string Status { get; set; } = "";
    }
    public class ClassStatDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";
        public int StudentCount { get; set; }
        public int ExamCount { get; set; }
        public int ResourceCount { get; set; }
    }
}
