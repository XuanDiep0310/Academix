using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public int? OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
