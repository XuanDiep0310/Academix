using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Course
{
    public int CourseId { get; set; }

    public int? OrganizationId { get; set; }

    public string? Code { get; set; }

    public string Title { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
