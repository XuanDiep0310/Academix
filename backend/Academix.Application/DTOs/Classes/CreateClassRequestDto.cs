using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Classes
{
    public class CreateClassRequestDto
    {
        [Required(ErrorMessage = "Class name is required")]
        [MaxLength(255)]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Class code is required")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Z0-9-_]+$", ErrorMessage = "Class code must contain only uppercase letters, numbers, hyphens, and underscores")]
        public string ClassCode { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}
