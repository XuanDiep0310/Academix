using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("ExternalLogin")]
[Index("UserId", Name = "IX_ExternalLogin_User")]
[Index("Provider", "ProviderKey", Name = "UQ_ExternalLogin", IsUnique = true)]
public partial class ExternalLogin
{
    [Key]
    public int ExternalLoginId { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    public string Provider { get; set; } = null!;

    [StringLength(200)]
    public string ProviderKey { get; set; } = null!;

    [StringLength(200)]
    public string? DisplayName { get; set; }

    public DateTime ConnectedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ExternalLogins")]
    public virtual User User { get; set; } = null!;
}
