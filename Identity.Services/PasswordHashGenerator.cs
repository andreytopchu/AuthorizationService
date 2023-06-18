using System.Security.Cryptography;
using System.Text;
using Identity.Application.Abstractions.Services;

namespace Identity.Services
{
    public class PasswordHashGenerator : IPasswordHashGenerator
    {
        public string MakeHash(string salt, string password)
        {
            if (string.IsNullOrWhiteSpace(salt))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(salt));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            var bytes = Encoding.UTF8.GetBytes(salt + password);

            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(bytes));
            }
        }
    }
}