using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("FileStorage")]
[Index("OrganizationId", Name = "IX_FileStorage_Org")]
[Index("UploadedBy", Name = "IX_FileStorage_Uploaded")]
public partial class FileStorage
{
    [Key]
    public int FileId { get; set; }

    public int? OrganizationId { get; set; }

    public int? UploadedBy { get; set; }

    [StringLength(500)]
    public string FileName { get; set; } = null!;

    [StringLength(2000)]
    public string? BlobUri { get; set; }

    [StringLength(100)]
    public string? StorageProvider { get; set; }

    [StringLength(200)]
    public string? ContentType { get; set; }

    public long? ContentLength { get; set; }

    [StringLength(128)]
    public string? Checksum { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("FileStorages")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<ResourceVersion> ResourceVersions { get; set; } = new List<ResourceVersion>();

    [InverseProperty("File")]
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    [ForeignKey("UploadedBy")]
    [InverseProperty("FileStorages")]
    public virtual User? UploadedByNavigation { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<WebcamCapture> WebcamCaptures { get; set; } = new List<WebcamCapture>();
}
