using Academix.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 32;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public (byte[] hash, byte[] salt) HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );
            return (hash, salt);
        }

        public bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );
            return CryptographicOperations.FixedTimeEquals(hashToCompare, hash);
        }
    }
}
