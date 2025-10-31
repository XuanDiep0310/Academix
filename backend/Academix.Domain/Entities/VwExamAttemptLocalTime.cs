using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class VwExamAttemptLocalTime
{
    public long AttemptId { get; set; }

    public int ExamId { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = null!;

    public decimal? Score { get; set; }

    public DateTime StartedAtUtc { get; set; }

    public DateTime? StartedAtVn { get; set; }

    public DateTime? SubmittedAtUtc { get; set; }

    public DateTime? SubmittedAtVn { get; set; }
}
