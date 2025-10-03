using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("Like")]
[Index("EntityType", "EntityId", Name = "IX_Like_Entity")]
[Index("UserId", Name = "IX_Like_User")]
[Index("EntityType", "EntityId", "UserId", Name = "UQ_Like", IsUnique = true)]
public partial class Like
{
    [Key]
    public int LikeId { get; set; }

    [StringLength(100)]
    public string EntityType { get; set; } = null!;

    public int EntityId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Likes")]
    public virtual User User { get; set; } = null!;
}
