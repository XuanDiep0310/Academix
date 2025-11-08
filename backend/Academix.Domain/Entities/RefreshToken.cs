using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("Token", Name = "IX_RefreshTokens_Token")]
[Index("UserId", Name = "IX_RefreshTokens_UserId")]
[Index("Token", Name = "UQ__RefreshT__1EB4F817D9D1C46F", IsUnique = true)]
public partial class RefreshToken
{
    [Key]
    public int TokenId { get; set; }

    public int UserId { get; set; }

    [StringLength(500)]
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsRevoked { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RefreshTokens")]
    public virtual User User { get; set; } = null!;
}
