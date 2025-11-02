using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Question
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public int? OrganizationId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Stem { get; set; } = string.Empty;
        public string? Solution { get; set; }
        public byte? Difficulty { get; set; }
        public string? Metadata { get; set; }
        public byte TypeId { get; set; }
        public List<QuestionOptionResponse> Options { get; set; } = new();
    }
}
