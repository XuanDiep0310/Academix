using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Teacher
{
    public class CreateExamRequest
    {
        public string Title { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<int> QuestionIds { get; set; } = new();
    }
}
