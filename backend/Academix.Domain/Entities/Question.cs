using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Question
{
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

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual QuestionType Type { get; set; } = null!;
}
