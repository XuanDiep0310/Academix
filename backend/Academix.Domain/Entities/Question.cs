using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Question")]
[Index("OrganizationId", Name = "IX_Question_Org")]
[Index("TypeId", Name = "IX_Question_Type")]
public partial class Question
{
    [Key]
    public int QuestionId { get; set; }

    public int? OrganizationId { get; set; }

    public int? CreatedBy { get; set; }

    public byte TypeId { get; set; }

    public string Stem { get; set; } = null!;

    public string? Solution { get; set; }

    public byte? Difficulty { get; set; }

    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Questions")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Questions")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    [InverseProperty("Question")]
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    [ForeignKey("TypeId")]
    [InverseProperty("Questions")]
    public virtual QuestionType Type { get; set; } = null!;
}
