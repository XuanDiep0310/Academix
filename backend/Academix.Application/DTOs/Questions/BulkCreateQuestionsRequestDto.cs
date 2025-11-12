using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Questions
{
    public class BulkCreateQuestionsRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<CreateQuestionRequestDto> Questions { get; set; } = new();
    }
}
