using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Classes
{
    public class AddMembersResponseDto
    {
        public List<ClassMemberDto> AddedMembers { get; set; } = new();
        public List<FailedMemberDto> FailedMembers { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class FailedMemberDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
