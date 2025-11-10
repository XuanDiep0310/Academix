using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Users
{
    public class UpdateProfileRequestDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;
    }
}
