using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime AssignedAt { get; set; }

    public int? AssignedBy { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
