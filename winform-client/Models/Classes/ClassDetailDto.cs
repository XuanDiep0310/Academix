using System;
using System.Collections.Generic;

namespace Academix.WinApp.Models.Classes
{
    public class ClassDetailDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ClassMember> Teachers { get; set; } = new();
        public List<ClassMember> Students { get; set; } = new();
        public int TeacherCount { get; set; }
        public int StudentCount { get; set; }
    }
}


