using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("StudentExamAttempt")]
[Index("ExamId", Name = "IX_Attempt_Exam")]
[Index("Status", Name = "IX_Attempt_Status")]
[Index("StartedAt", "SubmittedAt", Name = "IX_Attempt_Time")]
[Index("UserId", Name = "IX_Attempt_User")]
public partial class StudentExamAttempt
{
    [Key]
    public long AttemptId { get; set; }

    public int ExamId { get; set; }

    public int UserId { get; set; }

    public int? ClassId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal? Score { get; set; }

    [Column("IPAddress")]
    [StringLength(100)]
    public string? Ipaddress { get; set; }

    public string? BrowserInfo { get; set; }

    public string? DeviceInfo { get; set; }

    public int FocusLostCount { get; set; }

    public bool IsCheatingSuspected { get; set; }

    [InverseProperty("Attempt")]
    public virtual ICollection<CheatingAlert> CheatingAlerts { get; set; } = new List<CheatingAlert>();

    [ForeignKey("ClassId")]
    [InverseProperty("StudentExamAttempts")]
    public virtual Class? Class { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("StudentExamAttempts")]
    public virtual Exam Exam { get; set; } = null!;

    [InverseProperty("Attempt")]
    public virtual ICollection<FocusLog> FocusLogs { get; set; } = new List<FocusLog>();

    [InverseProperty("Attempt")]
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    [ForeignKey("UserId")]
    [InverseProperty("StudentExamAttempts")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Attempt")]
    public virtual ICollection<WebcamCapture> WebcamCaptures { get; set; } = new List<WebcamCapture>();
}
