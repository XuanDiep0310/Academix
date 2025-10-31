using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Notification
{
    public long NotificationId { get; set; }

    public int? OrganizationId { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    public string? Type { get; set; }

    public string? Data { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual User? User { get; set; }
}
