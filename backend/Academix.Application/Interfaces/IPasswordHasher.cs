using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Interfaces
{
    public interface IPasswordHasher
    {
        (byte[] hash, byte[] salt) HashPassword(string password);
        bool VerifyPassword(string password, byte[] hash, byte[] salt);
    }
}
