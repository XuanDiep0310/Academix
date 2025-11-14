using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Exams
{
    public class SubmitExamRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one answer is required")]
        public List<SubmitAnswerRequestDto> Answers { get; set; } = new();
    }
}
