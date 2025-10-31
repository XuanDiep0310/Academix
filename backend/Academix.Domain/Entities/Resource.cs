using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Resource
{
    public int ResourceId { get; set; }

    public int? OrganizationId { get; set; }

    public int? CourseId { get; set; }

    public int? ClassId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ResourceType { get; set; } = null!;

    public int? CurrentVersionId { get; set; }

    public string Visibility { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual ICollection<ResourceVersion> ResourceVersions { get; set; } = new List<ResourceVersion>();
}
