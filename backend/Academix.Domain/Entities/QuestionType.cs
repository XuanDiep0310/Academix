using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academix.Domain.Entities;

[Table("QuestionType")]
[Index("Name", Name = "UQ__Question__737584F62577FA09", IsUnique = true)]
public partial class QuestionType
{
    [Key]
    public byte QuestionTypeId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    [InverseProperty("Type")]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
