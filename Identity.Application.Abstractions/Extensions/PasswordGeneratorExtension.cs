using Identity.Application.Abstractions.Services;

namespace Identity.Application.Abstractions.Extensions
{
    public static class PasswordGeneratorExtension
    {
        public static string MakeHashWithSalt(this IPasswordHashGenerator hashGenerator, Guid salt, string password)
        {
            if (hashGenerator == null) throw new ArgumentNullException(nameof(hashGenerator));

            return hashGenerator.MakeHash(salt.ToString("N"), password);
        }

        public static string MakeHashWithSalt(this IPasswordHashGenerator hashGenerator, string salt, string password)
        {
            if (hashGenerator == null) throw new ArgumentNullException(nameof(hashGenerator));
            if (salt == null) throw new ArgumentNullException(nameof(salt));

            return hashGenerator.MakeHash(salt, password);
        }
    }
}