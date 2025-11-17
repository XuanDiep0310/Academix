using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Classes
{
    public class ClassMember
    {
        public int ClassMemberId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
