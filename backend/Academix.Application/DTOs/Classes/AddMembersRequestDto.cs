using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Classes
{
    public class AddMembersRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one user ID is required")]
        public List<int> UserIds { get; set; } = new();
    }
}
