using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class ExamQuestion
{
    public int ExamQuestionId { get; set; }

    public int ExamId { get; set; }

    public int QuestionId { get; set; }

    public decimal Score { get; set; }

    public int OrderIndex { get; set; }

    public bool RandomizeOptions { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
