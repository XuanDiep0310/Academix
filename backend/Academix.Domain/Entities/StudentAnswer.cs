using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class StudentAnswer
{
    public long StudentAnswerId { get; set; }

    public long AttemptId { get; set; }

    public int QuestionId { get; set; }

    public int? SelectedOptionId { get; set; }

    public string? AnswerText { get; set; }

    public int? FileId { get; set; }

    public decimal? ScoreAwarded { get; set; }

    public bool AutoGraded { get; set; }

    public int? GradedBy { get; set; }

    public DateTime? GradedAt { get; set; }

    public string? Feedback { get; set; }

    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    public virtual FileStorage? File { get; set; }

    public virtual User? GradedByNavigation { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual QuestionOption? SelectedOption { get; set; }
}
