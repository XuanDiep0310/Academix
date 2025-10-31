using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class WebcamCapture
{
    public long CaptureId { get; set; }

    public long AttemptId { get; set; }

    public int UserId { get; set; }

    public int? FileId { get; set; }

    public DateTime CapturedAt { get; set; }

    public bool FaceDetected { get; set; }

    public decimal? MatchScore { get; set; }

    public string? Notes { get; set; }

    public virtual StudentExamAttempt Attempt { get; set; } = null!;

    public virtual FileStorage? File { get; set; }

    public virtual User User { get; set; } = null!;
}
