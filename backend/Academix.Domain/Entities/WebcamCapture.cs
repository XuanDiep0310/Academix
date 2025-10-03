using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("WebcamCapture")]
[Index("AttemptId", "CapturedAt", Name = "IX_WebcamCapture_Attempt")]
public partial class WebcamCapture
{
    [Key]
    public long CaptureId { get; set; }

    public long AttemptId { get; set; }

    public int UserId { get; set; }

    public int? FileId { get; set; }

    public DateTime CapturedAt { get; set; }

    public bool FaceDetected { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? MatchScore { get; set; }

    public string? Notes { get; set; }

    [ForeignKey("AttemptId")]
    [InverseProperty("WebcamCaptures")]
    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    [ForeignKey("FileId")]
    [InverseProperty("WebcamCaptures")]
    public virtual FileStorage? File { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("WebcamCaptures")]
    public virtual User User { get; set; } = null!;
}
