using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("ExamId", Name = "IX_ExamQuestions_ExamId")]
[Index("ExamId", "QuestionId", Name = "UQ__ExamQues__F9A9273C549A3579", IsUnique = true)]
public partial class ExamQuestion
{
    [Key]
    public int ExamQuestionId { get; set; }

    public int ExamId { get; set; }

    public int QuestionId { get; set; }

    public int QuestionOrder { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Marks { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("ExamQuestions")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("QuestionId")]
    [InverseProperty("ExamQuestions")]
    public virtual Question Question { get; set; } = null!;
}
