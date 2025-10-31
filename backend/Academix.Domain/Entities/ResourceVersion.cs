using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class ResourceVersion
{
    public int ResourceVersionId { get; set; }

    public int ResourceId { get; set; }

    public int? FileId { get; set; }

    public int VersionNumber { get; set; }

    public string? Notes { get; set; }

    public int? DurationSeconds { get; set; }

    public long? SizeBytes { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual FileStorage? File { get; set; }

    public virtual Resource Resource { get; set; } = null!;
}
