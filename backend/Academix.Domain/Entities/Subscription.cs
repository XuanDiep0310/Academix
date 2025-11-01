using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public int OrganizationId { get; set; }

    public string PlanCode { get; set; } = null!;

    public int Seats { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
