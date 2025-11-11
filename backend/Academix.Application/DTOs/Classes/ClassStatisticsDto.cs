using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Classes
{
    public class ClassStatisticsDto
    {
        public int TotalClasses { get; set; }
        public int ActiveClasses { get; set; }
        public int InactiveClasses { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public double AverageStudentsPerClass { get; set; }
        public List<ClassGrowthDto> ClassGrowth { get; set; } = new();
    }

    public class ClassGrowthDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
