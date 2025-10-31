using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class FocusLog
{
    public long FocusLogId { get; set; }

    public long AttemptId { get; set; }

    public int UserId { get; set; }

    public DateTime OccurredAt { get; set; }

    public int? DurationSeconds { get; set; }

    public string? WindowTitle { get; set; }

    public string? Details { get; set; }

    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
