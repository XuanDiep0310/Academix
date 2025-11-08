using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("ClassCode", Name = "IX_Classes_ClassCode")]
[Index("ClassCode", Name = "UQ__Classes__2ECD4A55E0E24C85", IsUnique = true)]
public partial class Class
{
    [Key]
    public int ClassId { get; set; }

    [StringLength(255)]
    public string ClassName { get; set; } = null!;

    [StringLength(50)]
    public string ClassCode { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Class")]
    public virtual ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("Classes")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Class")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Class")]
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
