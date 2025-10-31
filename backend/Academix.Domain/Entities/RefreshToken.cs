using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public string? ReasonRevoked { get; set; }

    public int IsExpired { get; set; }

    public int IsRevoked { get; set; }

    public int IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
