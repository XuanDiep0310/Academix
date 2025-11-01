using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Domain { get; set; }

    public string? LogoUrl { get; set; }

    public string? ThemeJson { get; set; }

    public string? BillingContact { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<FileStorage> FileStorages { get; set; } = new List<FileStorage>();

    public virtual ICollection<GamificationPoint> GamificationPoints { get; set; } = new List<GamificationPoint>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
