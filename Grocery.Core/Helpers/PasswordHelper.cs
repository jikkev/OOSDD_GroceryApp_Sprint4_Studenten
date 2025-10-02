using System.Security.Cryptography;
using System.Text;

namespace Grocery.Core.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password must not be null or empty.", nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string? password, string? storedHash)
        {
            // null of leeg → altijd ongeldig
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                return false;

            try
            {
                var parts = storedHash.Split('.');
                if (parts.Length != 2) return false;

                var salt = Convert.FromBase64String(parts[0]);
                var hash = Convert.FromBase64String(parts[1]);

                if (salt.Length == 0 || hash.Length == 0)
                    return false;

                var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    salt,
                    100000,
                    HashAlgorithmName.SHA256,
                    32);

#if NET6_0_OR_GREATER
                return CryptographicOperations.FixedTimeEquals(inputHash, hash);
#else
                return inputHash.SequenceEqual(hash);
#endif
            }
            catch
            {

                return false;
            }
        }
    }
}
