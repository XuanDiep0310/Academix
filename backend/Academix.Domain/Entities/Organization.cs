using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Organization")]
public partial class Organization
{
    [Key]
    public int OrganizationId { get; set; }

    [StringLength(250)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Domain { get; set; }

    [StringLength(1000)]
    public string? LogoUrl { get; set; }

    public string? ThemeJson { get; set; }

    [StringLength(200)]
    public string? BillingContact { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("Organization")]
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    [InverseProperty("Organization")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("Organization")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Organization")]
    public virtual ICollection<FileStorage> FileStorages { get; set; } = new List<FileStorage>();

    [InverseProperty("Organization")]
    public virtual ICollection<GamificationPoint> GamificationPoints { get; set; } = new List<GamificationPoint>();

    [InverseProperty("Organization")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("Organization")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Organization")]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    [InverseProperty("Organization")]
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    [InverseProperty("Organization")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    [InverseProperty("Organization")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    [InverseProperty("Organization")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
