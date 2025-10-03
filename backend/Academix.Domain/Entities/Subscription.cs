using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Subscription")]
[Index("OrganizationId", "Status", Name = "IX_Subscription_Org")]
[Index("StartAt", "EndAt", Name = "IX_Subscription_Period")]
public partial class Subscription
{
    [Key]
    public int SubscriptionId { get; set; }

    public int OrganizationId { get; set; }

    [StringLength(100)]
    public string PlanCode { get; set; } = null!;

    public int Seats { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Subscriptions")]
    public virtual Organization Organization { get; set; } = null!;
}
