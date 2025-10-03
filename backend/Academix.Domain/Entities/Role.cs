using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Role")]
[Index("OrganizationId", "Name", Name = "UQ_Role_Name", IsUnique = true)]
public partial class Role
{
    [Key]
    public int RoleId { get; set; }

    public int? OrganizationId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Roles")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    [InverseProperty("Role")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
