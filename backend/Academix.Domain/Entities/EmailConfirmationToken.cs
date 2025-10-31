using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class EmailConfirmationToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public int IsExpired { get; set; }

    public int IsUsed { get; set; }

    public bool IsValid => IsExpired == 0 && IsUsed == 0;

    public virtual User User { get; set; } = null!;
}
