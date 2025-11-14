using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Materials
{
    public class CreateMaterialRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Material type is required")]
        public string MaterialType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? FileUrl { get; set; }

        [MaxLength(255)]
        public string? FileName { get; set; }

        public long? FileSize { get; set; }
    }
}
