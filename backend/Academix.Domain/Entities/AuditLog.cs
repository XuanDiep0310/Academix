using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class AuditLog
{
    public long AuditId { get; set; }

    public int? UserId { get; set; }

    public int? OrganizationId { get; set; }

    public string Action { get; set; } = null!;

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public string? Detail { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual User? User { get; set; }
}
