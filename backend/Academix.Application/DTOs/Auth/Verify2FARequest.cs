using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.DTOs.Auth
{
    public class Verify2FARequest
    {
        public string Code { get; set; } = string.Empty;
    }
}
