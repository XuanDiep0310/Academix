using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class TokenBlacklist
{
    public int TokenBlacklistId { get; set; }

    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime RevokedAt { get; set; }

    public string? Reason { get; set; }
}
