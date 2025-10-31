using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class StudentExamAttempt
{
    public long AttemptId { get; set; }

    public int ExamId { get; set; }

    public int UserId { get; set; }

    public int? ClassId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string Status { get; set; } = null!;

    public decimal? Score { get; set; }

    public string? Ipaddress { get; set; }

    public string? BrowserInfo { get; set; }

    public string? DeviceInfo { get; set; }

    public int FocusLostCount { get; set; }

    public bool IsCheatingSuspected { get; set; }

    public virtual ICollection<CheatingAlert> CheatingAlerts { get; set; } = new List<CheatingAlert>();

    public virtual Class? Class { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<FocusLog> FocusLogs { get; set; } = new List<FocusLog>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WebcamCapture> WebcamCaptures { get; set; } = new List<WebcamCapture>();
}
