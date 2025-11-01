using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public int? OrganizationId { get; set; }

    public string Email { get; set; } = null!;

    public string NormalizedEmail { get; set; } = null!;

    public string? UserName { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public bool TwoFaenabled { get; set; }

    public string? TwoFasecret { get; set; }

    public string? DisplayName { get; set; }

    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    /// <summary>
    /// Lưu thời gian tạo theo UTC (Coordinated Universal Time). KHÔNG dùng giờ địa phương.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Lưu lần login cuối theo UTC. Convert sang giờ địa phương khi hiển thị.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<CheatingAlert> CheatingAlertHandledByNavigations { get; set; } = new List<CheatingAlert>();

    public virtual ICollection<CheatingAlert> CheatingAlertUsers { get; set; } = new List<CheatingAlert>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = new List<EmailConfirmationToken>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

    public virtual ICollection<FileStorage> FileStorages { get; set; } = new List<FileStorage>();

    public virtual ICollection<FocusLog> FocusLogs { get; set; } = new List<FocusLog>();

    public virtual ICollection<GamificationPoint> GamificationPoints { get; set; } = new List<GamificationPoint>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<ResourceVersion> ResourceVersions { get; set; } = new List<ResourceVersion>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<WebcamCapture> WebcamCaptures { get; set; } = new List<WebcamCapture>();
}
