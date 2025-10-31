using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class FileStorage
{
    public int FileId { get; set; }

    public int? OrganizationId { get; set; }

    public int? UploadedBy { get; set; }

    public string FileName { get; set; } = null!;

    public string? BlobUri { get; set; }

    public string? StorageProvider { get; set; }

    public string? ContentType { get; set; }

    public long? ContentLength { get; set; }

    public string? Checksum { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<ResourceVersion> ResourceVersions { get; set; } = new List<ResourceVersion>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual User? UploadedByNavigation { get; set; }

    public virtual ICollection<WebcamCapture> WebcamCaptures { get; set; } = new List<WebcamCapture>();
}
