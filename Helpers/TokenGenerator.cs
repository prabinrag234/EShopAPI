using System.Security.Cryptography;

namespace EShopAPI.Helpers
{
    public static class TokenGenerator
    {
        public static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}