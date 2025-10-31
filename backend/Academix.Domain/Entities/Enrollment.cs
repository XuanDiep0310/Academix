using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public int UserId { get; set; }

    public string RoleInClass { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public bool IsApproved { get; set; }

    public bool IsActive { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
