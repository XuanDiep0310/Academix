using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Teacher
{
    public class ExamDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int DurationMinutes { get; set; }
        public int QuestionCount { get; set; }
        public string Status { get; set; } = string.Empty;


    }
}
