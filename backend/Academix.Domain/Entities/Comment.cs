using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Comment")]
[Index("EntityType", "EntityId", Name = "IX_Comment_Entity")]
[Index("ParentCommentId", Name = "IX_Comment_Parent")]
[Index("UserId", Name = "IX_Comment_User")]
public partial class Comment
{
    [Key]
    public int CommentId { get; set; }

    [StringLength(100)]
    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public int? ParentCommentId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("ParentComment")]
    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    [ForeignKey("ParentCommentId")]
    [InverseProperty("InverseParentComment")]
    public virtual Comment? ParentComment { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Comments")]
    public virtual User User { get; set; } = null!;
}
