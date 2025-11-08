using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("TeacherId", Name = "IX_Questions_TeacherId")]
public partial class Question
{
    [Key]
    public int QuestionId { get; set; }

    public int TeacherId { get; set; }

    public string QuestionText { get; set; } = null!;

    [StringLength(50)]
    public string? QuestionType { get; set; }

    [StringLength(20)]
    public string? DifficultyLevel { get; set; }

    [StringLength(255)]
    public string? Subject { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    [InverseProperty("Question")]
    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    [InverseProperty("Question")]
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    [ForeignKey("TeacherId")]
    [InverseProperty("Questions")]
    public virtual User Teacher { get; set; } = null!;
}
