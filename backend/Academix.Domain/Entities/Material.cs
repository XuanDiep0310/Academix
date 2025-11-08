using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Index("ClassId", Name = "IX_Materials_ClassId")]
[Index("UploadedBy", Name = "IX_Materials_UploadedBy")]
public partial class Material
{
    [Key]
    public int MaterialId { get; set; }

    public int ClassId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string MaterialType { get; set; } = null!;

    [StringLength(1000)]
    public string? FileUrl { get; set; }

    [StringLength(255)]
    public string? FileName { get; set; }

    public long? FileSize { get; set; }

    public int UploadedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Materials")]
    public virtual Class Class { get; set; } = null!;

    [ForeignKey("UploadedBy")]
    [InverseProperty("Materials")]
    public virtual User UploadedByNavigation { get; set; } = null!;
}
