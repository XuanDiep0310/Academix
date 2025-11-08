using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Keyless]
public partial class VwExamResult
{
    public int AttemptId { get; set; }

    public int ExamId { get; set; }

    [StringLength(255)]
    public string ExamTitle { get; set; } = null!;

    public int StudentId { get; set; }

    [StringLength(255)]
    public string StudentName { get; set; } = null!;

    [StringLength(255)]
    public string StudentEmail { get; set; } = null!;

    public DateTime? StartTime { get; set; }

    public DateTime? SubmitTime { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalScore { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalMarks { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public int? TotalAnswered { get; set; }

    public int? CorrectAnswers { get; set; }
}
