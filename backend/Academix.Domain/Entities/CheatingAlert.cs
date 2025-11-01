using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class CheatingAlert
{
    public long CheatingAlertId { get; set; }

    public long? AttemptId { get; set; }

    public int UserId { get; set; }

    public string AlertType { get; set; } = null!;

    public byte Severity { get; set; }

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? HandledBy { get; set; }

    public DateTime? HandledAt { get; set; }

    public virtual StudentExamAttempt? Attempt { get; set; }

    public virtual User? HandledByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
