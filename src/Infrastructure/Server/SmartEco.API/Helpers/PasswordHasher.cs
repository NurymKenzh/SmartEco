using Microsoft.AspNetCore.Identity;

namespace SmartEco.API.Helpers
{
    public static class PasswordHasher
    {
        private static readonly PasswordHasher<string> _hasher = new();

        public static string GetHash(string password)
            => _hasher.HashPassword(string.Empty, password);

        public static bool IsNotVerify(string password, string hashedPassword)
        {
            var result = _hasher.VerifyHashedPassword(string.Empty, hashedPassword, password);
            return result is not PasswordVerificationResult.Success;
        }
    }
}
