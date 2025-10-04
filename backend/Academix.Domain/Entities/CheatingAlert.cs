using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("CheatingAlert")]
[Index("AttemptId", Name = "IX_CheatingAlert_Attempt")]
[Index("HandledAt", Name = "IX_CheatingAlert_Status")]
[Index("UserId", Name = "IX_CheatingAlert_User")]
public partial class CheatingAlert
{
    [Key]
    public long CheatingAlertId { get; set; }

    public long? AttemptId { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    public string AlertType { get; set; } = null!;

    public byte Severity { get; set; }

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? HandledBy { get; set; }

    public DateTime? HandledAt { get; set; }

    [ForeignKey("AttemptId")]
    [InverseProperty("CheatingAlerts")]
    public virtual StudentExamAttempt? Attempt { get; set; }

    [ForeignKey("HandledBy")]
    [InverseProperty("CheatingAlertHandledByNavigations")]
    public virtual User? HandledByNavigation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CheatingAlertUsers")]
    public virtual User User { get; set; } = null!;
}
