using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities;

public partial class QuestionType
{
    public byte QuestionTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
