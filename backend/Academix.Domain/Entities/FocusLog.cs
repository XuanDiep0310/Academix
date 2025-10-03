using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("FocusLog")]
[Index("AttemptId", "OccurredAt", Name = "IX_FocusLog_Attempt")]
[Index("UserId", Name = "IX_FocusLog_User")]
public partial class FocusLog
{
    [Key]
    public long FocusLogId { get; set; }

    public long AttemptId { get; set; }

    public int UserId { get; set; }

    public DateTime OccurredAt { get; set; }

    public int? DurationSeconds { get; set; }

    [StringLength(1000)]
    public string? WindowTitle { get; set; }

    public string? Details { get; set; }

    [ForeignKey("AttemptId")]
    [InverseProperty("FocusLogs")]
    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("FocusLogs")]
    public virtual User User { get; set; } = null!;
}
