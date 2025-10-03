using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Progress")]
[Index("ResourceId", Name = "IX_Progress_Resource")]
[Index("UserId", Name = "IX_Progress_User")]
[Index("UserId", "ResourceId", Name = "UQ_Progress", IsUnique = true)]
public partial class Progress
{
    [Key]
    public long ProgressId { get; set; }

    public int UserId { get; set; }

    public int ResourceId { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal WatchedPercentage { get; set; }

    public bool Completed { get; set; }

    public DateTime LastSeenAt { get; set; }

    [ForeignKey("ResourceId")]
    [InverseProperty("Progresses")]
    public virtual Resource Resource { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Progresses")]
    public virtual User User { get; set; } = null!;
}
