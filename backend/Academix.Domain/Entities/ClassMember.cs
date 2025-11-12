using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("ClassId", Name = "IX_ClassMembers_ClassId")]
[Index("UserId", Name = "IX_ClassMembers_UserId")]
[Index("ClassId", "UserId", Name = "UQ__ClassMem__1A61AB055A5534E1", IsUnique = true)]
public partial class ClassMember
{
    [Key]
    public int ClassMemberId { get; set; }

    public int ClassId { get; set; }

    public int UserId { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("ClassMembers")]
    public virtual Class Class { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ClassMembers")]
    public virtual User User { get; set; } = null!;
}
