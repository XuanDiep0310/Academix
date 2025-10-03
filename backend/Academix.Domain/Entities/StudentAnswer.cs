using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("StudentAnswer")]
[Index("AttemptId", Name = "IX_StudentAnswer_Attempt")]
[Index("QuestionId", Name = "IX_StudentAnswer_Question")]
[Index("AttemptId", "QuestionId", Name = "UQ_StudentAnswer", IsUnique = true)]
public partial class StudentAnswer
{
    [Key]
    public long StudentAnswerId { get; set; }

    public long AttemptId { get; set; }

    public int QuestionId { get; set; }

    public int? SelectedOptionId { get; set; }

    public string? AnswerText { get; set; }

    public int? FileId { get; set; }

    [Column(TypeName = "decimal(6, 2)")]
    public decimal? ScoreAwarded { get; set; }

    public bool AutoGraded { get; set; }

    public int? GradedBy { get; set; }

    public DateTime? GradedAt { get; set; }

    public string? Feedback { get; set; }

    [ForeignKey("AttemptId")]
    [InverseProperty("StudentAnswers")]
    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    [ForeignKey("FileId")]
    [InverseProperty("StudentAnswers")]
    public virtual FileStorage? File { get; set; }

    [ForeignKey("GradedBy")]
    [InverseProperty("StudentAnswers")]
    public virtual User? GradedByNavigation { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("StudentAnswers")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("SelectedOptionId")]
    [InverseProperty("StudentAnswers")]
    public virtual QuestionOption? SelectedOption { get; set; }
}
