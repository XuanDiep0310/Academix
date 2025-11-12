using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Materials
{
    public class UpdateMaterialRequestDto
    {
        [MaxLength(255)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? FileUrl { get; set; }
    }
}
