using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Course
{
    public class UpdateCourseRequest
    {
        public int? OrganizationId { get; set; }
        public string? Code { get; set; }
        public string Title { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public bool IsPublished { get; set; }
    }
}
