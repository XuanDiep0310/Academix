using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Keyless]
public partial class VwUserLocalTime
{
    public int UserId { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(200)]
    public string? DisplayName { get; set; }

    public bool IsActive { get; set; }

    [Column("CreatedAtUTC")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("CreatedAtVN")]
    public DateTime? CreatedAtVn { get; set; }

    [Column("LastLoginAtUTC")]
    public DateTime? LastLoginAtUtc { get; set; }

    [Column("LastLoginAtVN")]
    public DateTime? LastLoginAtVn { get; set; }
}
