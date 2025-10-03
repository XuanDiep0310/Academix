using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Enrollment")]
[Index("ClassId", "UserId", "IsActive", Name = "IX_Enrollment_Active")]
[Index("ClassId", Name = "IX_Enrollment_Class")]
[Index("UserId", Name = "IX_Enrollment_User")]
[Index("ClassId", "UserId", Name = "UQ_Enrollment", IsUnique = true)]
public partial class Enrollment
{
    [Key]
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public int UserId { get; set; }

    [StringLength(50)]
    public string RoleInClass { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public bool IsApproved { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Enrollments")]
    public virtual Class Class { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Enrollments")]
    public virtual User User { get; set; } = null!;
}
