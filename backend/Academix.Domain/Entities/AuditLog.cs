using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("AuditLog")]
[Index("EntityType", "EntityId", Name = "IX_AuditLog_Entity")]
[Index("CreatedAt", Name = "IX_AuditLog_Time")]
[Index("UserId", "CreatedAt", Name = "IX_AuditLog_User")]
public partial class AuditLog
{
    [Key]
    public long AuditId { get; set; }

    public int? UserId { get; set; }

    public int? OrganizationId { get; set; }

    [StringLength(200)]
    public string Action { get; set; } = null!;

    [StringLength(200)]
    public string? EntityType { get; set; }

    [StringLength(200)]
    public string? EntityId { get; set; }

    public string? Detail { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("AuditLogs")]
    public virtual Organization? Organization { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogs")]
    public virtual User? User { get; set; }
}
