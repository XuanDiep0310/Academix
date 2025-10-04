using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Course")]
[Index("OrganizationId", "Code", Name = "UQ_Course_Code", IsUnique = true)]
public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    public int? OrganizationId { get; set; }

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(300)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("Courses")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Courses")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
