using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Progress
{
    public long ProgressId { get; set; }

    public int UserId { get; set; }

    public int ResourceId { get; set; }

    public decimal WatchedPercentage { get; set; }

    public bool Completed { get; set; }

    public DateTime LastSeenAt { get; set; }

    public virtual Resource Resource { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
