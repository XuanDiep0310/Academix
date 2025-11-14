using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Models.Users
{
    public class UserBulkRequest
    {
        public List<UserCreateRequest> Users { get; set; } = new();
    }
}
