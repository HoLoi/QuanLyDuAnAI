using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace QuanLyDuAn.Services
{
    public static class AccountActivationTokenHelper
    {
        public const string LoginProvider = "QuanLyDuAn";
        public const string TokenName = "AccountActivation";

        public static (string TokenForUrl, AccountActivationTokenPayload Payload) CreatePayload(TimeSpan lifetime)
        {
            var tokenForUrl = TaoTokenUrlSafe();
            var now = DateTime.UtcNow;

            return (tokenForUrl, new AccountActivationTokenPayload
            {
                TokenHash = HashToken(tokenForUrl),
                CreatedAtUtc = now,
                ExpiresAtUtc = now.Add(lifetime)
            });
        }

        public static string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }

        public static bool IsTokenMatch(string expectedHash, string token)
        {
            var actualHash = HashToken(token);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedHash);
            var actualBytes = Encoding.UTF8.GetBytes(actualHash);
            return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
        }

        public static string Serialize(AccountActivationTokenPayload payload)
        {
            return JsonSerializer.Serialize(payload);
        }

        public static AccountActivationTokenPayload? Deserialize(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<AccountActivationTokenPayload>(json);
            }
            catch
            {
                return null;
            }
        }

        private static string TaoTokenUrlSafe()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }

    public class AccountActivationTokenPayload
    {
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
