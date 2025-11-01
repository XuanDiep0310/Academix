using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Exam
{
    public int ExamId { get; set; }

    public int? OrganizationId { get; set; }

    public int? CourseId { get; set; }

    public int? ClassId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? DurationMinutes { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public bool ShuffleQuestions { get; set; }

    public bool AllowBackNavigation { get; set; }

    public bool ProctoringRequired { get; set; }

    public byte AntiCheatLevel { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();
}
