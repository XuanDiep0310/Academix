using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Student
{
    public class StudentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public double? AvgScore { get; set; }  // có thể là null
        public int ExamCount { get; set; }
    }

}
