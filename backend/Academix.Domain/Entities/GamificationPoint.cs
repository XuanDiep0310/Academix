using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("GamificationPoint")]
[Index("UserId", "CreatedAt", Name = "IX_GamificationPoint_User")]
public partial class GamificationPoint
{
    [Key]
    public long PointId { get; set; }

    public int UserId { get; set; }

    public int? OrganizationId { get; set; }

    [StringLength(100)]
    public string ActivityType { get; set; } = null!;

    public int Points { get; set; }

    [StringLength(100)]
    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("GamificationPoints")]
    public virtual Organization? Organization { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("GamificationPoints")]
    public virtual User User { get; set; } = null!;
}
