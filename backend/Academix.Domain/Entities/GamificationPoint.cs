using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class GamificationPoint
{
    public long PointId { get; set; }

    public int UserId { get; set; }

    public int? OrganizationId { get; set; }

    public string ActivityType { get; set; } = null!;

    public int Points { get; set; }

    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual User User { get; set; } = null!;
}
