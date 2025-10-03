using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Payment")]
[Index("OrganizationId", "CreatedAt", Name = "IX_Payment_Org")]
[Index("Provider", "ProviderTransactionId", Name = "IX_Payment_Provider")]
public partial class Payment
{
    [Key]
    public long PaymentId { get; set; }

    public int OrganizationId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(100)]
    public string? Provider { get; set; }

    [StringLength(200)]
    public string? ProviderTransactionId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Payments")]
    public virtual Organization Organization { get; set; } = null!;
}
