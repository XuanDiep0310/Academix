using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("ClassId", Name = "IX_Exams_ClassId")]
[Index("CreatedBy", Name = "IX_Exams_CreatedBy")]
public partial class Exam
{
    [Key]
    public int ExamId { get; set; }

    public int ClassId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int Duration { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalMarks { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsPublished { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Exams")]
    public virtual Class Class { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("Exams")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Exam")]
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    [InverseProperty("Exam")]
    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();
}
