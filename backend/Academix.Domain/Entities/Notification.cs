using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Notification")]
[Index("UserId", "IsRead", "CreatedAt", Name = "IX_Notification_User")]
public partial class Notification
{
    [Key]
    public long NotificationId { get; set; }

    public int? OrganizationId { get; set; }

    public int? UserId { get; set; }

    [StringLength(300)]
    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    [StringLength(100)]
    public string? Type { get; set; }

    public string? Data { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Notifications")]
    public virtual Organization? Organization { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User? User { get; set; }
}
