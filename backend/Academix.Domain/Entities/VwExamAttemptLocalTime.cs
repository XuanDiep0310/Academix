using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Keyless]
public partial class VwExamAttemptLocalTime
{
    public long AttemptId { get; set; }

    public int ExamId { get; set; }

    public int UserId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal? Score { get; set; }

    [Column("StartedAtUTC")]
    public DateTime StartedAtUtc { get; set; }

    [Column("StartedAtVN")]
    public DateTime? StartedAtVn { get; set; }

    [Column("SubmittedAtUTC")]
    public DateTime? SubmittedAtUtc { get; set; }

    [Column("SubmittedAtVN")]
    public DateTime? SubmittedAtVn { get; set; }
}
