using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class Comment
{
    public int CommentId { get; set; }

    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public int? ParentCommentId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual User User { get; set; } = null!;
}
