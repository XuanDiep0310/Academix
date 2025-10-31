using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Class
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

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();

    public virtual User? Teacher { get; set; }
}
