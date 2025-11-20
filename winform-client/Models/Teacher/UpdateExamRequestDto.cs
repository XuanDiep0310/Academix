using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Teacher
{
    public class UpdateExamRequestDto
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? Duration { get; set; }

        public decimal? TotalMarks { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

}
