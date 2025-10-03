using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("User")]
[Index("OrganizationId", "NormalizedEmail", Name = "IX_User_Email")]
[Index("OrganizationId", "NormalizedEmail", Name = "UQ_User_Email", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    public int? OrganizationId { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(256)]
    public string NormalizedEmail { get; set; } = null!;

    [StringLength(100)]
    public string? UserName { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [Column("TwoFAEnabled")]
    public bool TwoFaenabled { get; set; }

    [Column("TwoFASecret")]
    [StringLength(512)]
    public string? TwoFasecret { get; set; }

    [StringLength(200)]
    public string? DisplayName { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(1000)]
    public string? AvatarUrl { get; set; }

    [StringLength(1000)]
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

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("HandledByNavigation")]
    public virtual ICollection<CheatingAlert> CheatingAlertHandledByNavigations { get; set; } = new List<CheatingAlert>();

    [InverseProperty("User")]
    public virtual ICollection<CheatingAlert> CheatingAlertUsers { get; set; } = new List<CheatingAlert>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    [InverseProperty("User")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("User")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("User")]
    public virtual ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

    [InverseProperty("UploadedByNavigation")]
    public virtual ICollection<FileStorage> FileStorages { get; set; } = new List<FileStorage>();

    [InverseProperty("User")]
    public virtual ICollection<FocusLog> FocusLogs { get; set; } = new List<FocusLog>();

    [InverseProperty("User")]
    public virtual ICollection<GamificationPoint> GamificationPoints { get; set; } = new List<GamificationPoint>();

    [InverseProperty("User")]
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Users")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<ResourceVersion> ResourceVersions { get; set; } = new List<ResourceVersion>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    [InverseProperty("GradedByNavigation")]
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    [InverseProperty("User")]
    public virtual ICollection<StudentExamAttempt> StudentExamAttempts { get; set; } = new List<StudentExamAttempt>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    [InverseProperty("User")]
    public virtual ICollection<WebcamCapture> WebcamCaptures { get; set; } = new List<WebcamCapture>();
}
