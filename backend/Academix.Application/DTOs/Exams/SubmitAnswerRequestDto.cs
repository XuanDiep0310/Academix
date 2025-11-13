using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Exams
{
    public class SubmitAnswerRequestDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int SelectedOptionId { get; set; }
    }
}
