using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Keyless]
public partial class VwClassSummary
{
    public int ClassId { get; set; }

    [StringLength(255)]
    public string ClassName { get; set; } = null!;

    [StringLength(50)]
    public string ClassCode { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? TeacherCount { get; set; }

    public int? StudentCount { get; set; }
}
