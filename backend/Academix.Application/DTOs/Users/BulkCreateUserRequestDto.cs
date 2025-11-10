using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Users
{
    public class BulkCreateUserRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one user is required")]
        public List<CreateUserRequestDto> Users { get; set; } = new();
    }
}
