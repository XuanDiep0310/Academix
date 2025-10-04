using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Class")]
[Index("CourseId", Name = "IX_Class_Course")]
[Index("TeacherId", Name = "IX_Class_Teacher")]
public partial class Class
{
    [Key]
    public int ClassId { get; set; }

    public int CourseId { get; set; }

    public int? OrganizationId { get; set; }

    [StringLength(300)]
    public string Title { get; set; } = null!;

    public int? TeacherId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [StringLength(50)]
    public string? EnrollmentCode { get; set; }

    public int? MaxStudents { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Classes")]
    public virtual Course Course { get; set; } = null!;

    [InverseProperty("Class")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [InverseProperty("Class")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Classes")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Class")]
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    [InverseProperty("Class")]
    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();

    [ForeignKey("TeacherId")]
    [InverseProperty("Classes")]
    public virtual User? Teacher { get; set; }
}
