using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class VwUserLocalTime
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? CreatedAtVn { get; set; }

    public DateTime? LastLoginAtUtc { get; set; }

    public DateTime? LastLoginAtVn { get; set; }
}
