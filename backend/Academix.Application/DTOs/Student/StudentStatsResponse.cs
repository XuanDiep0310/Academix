using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Student
{
    public class StudentStatsResponse
    {
        public int StudentId { get; set; }
        public int ExamCount { get; set; }
        public double? AverageScore { get; set; }  // kiểu double?
    }
}
