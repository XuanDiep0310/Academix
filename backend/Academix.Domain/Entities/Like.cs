using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Like
{
    public int LikeId { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
