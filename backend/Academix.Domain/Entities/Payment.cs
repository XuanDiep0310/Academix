using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Payment
{
    public long PaymentId { get; set; }

    public int OrganizationId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string? Provider { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
