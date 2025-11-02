using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Question
{
    public class QuestionOptionResponse
    {
        public int Id { get; set; }               // tương ứng OptionId
        public string Content { get; set; } = ""; // tương ứng Text
        public bool IsCorrect { get; set; }
        public int OrderIndex { get; set; }
    }
}
