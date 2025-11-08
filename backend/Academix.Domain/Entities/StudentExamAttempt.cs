using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("ExamId", Name = "IX_StudentExamAttempts_ExamId")]
[Index("Status", Name = "IX_StudentExamAttempts_Status")]
[Index("StudentId", Name = "IX_StudentExamAttempts_StudentId")]
public partial class StudentExamAttempt
{
    [Key]
    public int AttemptId { get; set; }

    public int ExamId { get; set; }

    public int StudentId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? SubmitTime { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalScore { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("StudentExamAttempts")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("StudentExamAttempts")]
    public virtual User Student { get; set; } = null!;

    [InverseProperty("Attempt")]
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
