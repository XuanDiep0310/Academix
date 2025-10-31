using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class ExternalLogin
{
    public int ExternalLoginId { get; set; }

    public int UserId { get; set; }

    public string Provider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? DisplayName { get; set; }

    public DateTime ConnectedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
