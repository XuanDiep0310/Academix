using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Classes
{
    public class UpdateClassRequestDto
    {
        [MaxLength(255)]
        public string? ClassName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
