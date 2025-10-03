using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("ExamQuestion")]
[Index("ExamId", "OrderIndex", Name = "IX_ExamQuestion_Exam")]
[Index("ExamId", "QuestionId", Name = "UQ_ExamQuestion", IsUnique = true)]
public partial class ExamQuestion
{
    [Key]
    public int ExamQuestionId { get; set; }

    public int ExamId { get; set; }

    public int QuestionId { get; set; }

    [Column(TypeName = "decimal(6, 2)")]
    public decimal Score { get; set; }

    public int OrderIndex { get; set; }

    public bool RandomizeOptions { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("ExamQuestions")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("QuestionId")]
    [InverseProperty("ExamQuestions")]
    public virtual Question Question { get; set; } = null!;
}
