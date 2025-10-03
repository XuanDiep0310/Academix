using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("ResourceVersion")]
[Index("ResourceId", Name = "IX_ResourceVersion_Resource")]
[Index("ResourceId", "VersionNumber", Name = "UQ_ResourceVersion", IsUnique = true)]
public partial class ResourceVersion
{
    [Key]
    public int ResourceVersionId { get; set; }

    public int ResourceId { get; set; }

    public int? FileId { get; set; }

    public int VersionNumber { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public int? DurationSeconds { get; set; }

    public long? SizeBytes { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ResourceVersions")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("FileId")]
    [InverseProperty("ResourceVersions")]
    public virtual FileStorage? File { get; set; }

    [ForeignKey("ResourceId")]
    [InverseProperty("ResourceVersions")]
    public virtual Resource Resource { get; set; } = null!;
}
