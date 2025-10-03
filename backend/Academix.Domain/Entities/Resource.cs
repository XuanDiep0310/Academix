using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Resource")]
[Index("ClassId", Name = "IX_Resource_Class")]
[Index("CourseId", Name = "IX_Resource_Course")]
public partial class Resource
{
    [Key]
    public int ResourceId { get; set; }

    public int? OrganizationId { get; set; }

    public int? CourseId { get; set; }

    public int? ClassId { get; set; }

    [StringLength(500)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(50)]
    public string ResourceType { get; set; } = null!;

    public int? CurrentVersionId { get; set; }

    [StringLength(50)]
    public string Visibility { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Resources")]
    public virtual Class? Class { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Resources")]
    public virtual Course? Course { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Resources")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Resources")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Resource")]
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    [InverseProperty("Resource")]
    public virtual ICollection<ResourceVersion> ResourceVersions { get; set; } = new List<ResourceVersion>();
}
