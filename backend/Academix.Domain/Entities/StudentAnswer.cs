using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("AttemptId", Name = "IX_StudentAnswers_AttemptId")]
public partial class StudentAnswer
{
    [Key]
    public int AnswerId { get; set; }

    public int AttemptId { get; set; }

    public int QuestionId { get; set; }

    public int? SelectedOptionId { get; set; }

    public bool? IsCorrect { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? MarksObtained { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("AttemptId")]
    [InverseProperty("StudentAnswers")]
    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    [ForeignKey("QuestionId")]
    [InverseProperty("StudentAnswers")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("SelectedOptionId")]
    [InverseProperty("StudentAnswers")]
    public virtual QuestionOption? SelectedOption { get; set; }
}
