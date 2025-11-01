using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Class
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int? OrganizationId { get; set; }
        public string Title { get; set; } = null!;
        public int? TeacherId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? EnrollmentCode { get; set; }
        public int? MaxStudents { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
