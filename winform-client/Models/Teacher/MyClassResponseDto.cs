using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class MyClassResponseDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        //public string? Description { get; set; }
        //public string MyRole { get; set; } = string.Empty; // Teacher or Student
        //public DateTime JoinedAt { get; set; }
        //public int TeacherCount { get; set; }
        public int StudentCount { get; set; }
        //public bool IsActive { get; set; }
    }
}
