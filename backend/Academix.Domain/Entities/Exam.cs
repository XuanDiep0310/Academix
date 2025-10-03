using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Exam")]
[Index("ClassId", Name = "IX_Exam_Class")]
[Index("CourseId", Name = "IX_Exam_Course")]
[Index("StartAt", "EndAt", Name = "IX_Exam_Schedule")]
public partial class Exam
{
    [Key]
    public int ExamId { get; set; }

    public int? OrganizationId { get; set; }

    public int? CourseId { get; set; }

    public int? ClassId { get; set; }

    [StringLength(300)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? DurationMinutes { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public bool ShuffleQuestions { get; set; }

    public bool AllowBackNavigation { get; set; }

    public bool ProctoringRequired { get; set; }

    public byte AntiCheatLevel { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Exams")]
    public virtual Class? Class { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Exams")]
    public virtual Course? Course { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Exams")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Exams")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();
}
