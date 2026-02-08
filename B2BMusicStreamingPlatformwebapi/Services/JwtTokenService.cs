using API.Models.Identity;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    /// <summary>
    /// Service for generating JWT tokens for authentication.
    /// The JWT payload includes the TenantId for multi-tenant isolation.
    /// Uses Entity Framework Core with PostgreSQL for user and tenant data.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = configuration["Jwt:SecretKey"] ?? "your-secret-key-minimum-32-characters-long!!!";
        }

        /// <summary>
        /// Generates a simple JWT token for an authenticated user.
        /// The token payload encapsulates the TenantId for multi-tenant isolation.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <param name="tenantId">The tenant (venue) ID.</param>
        /// <returns>JWT token string.</returns>
        public string GenerateToken(ApplicationUser user, string tenantId)
        {
            // Create header
            var header = new { alg = "HS256", typ = "JWT" };
            var headerJson = System.Text.Json.JsonSerializer.Serialize(header);
            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerJson)).Replace("+", "-").Replace("/", "_").TrimEnd('=');

            // Create payload
            var expirationTime = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));
            var payload = new
            {
                nameid = user.Id,
                email = user.Email,
                name = user.FullName,
                TenantId = tenantId,
                role = user.Role,
                iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = new DateTimeOffset(expirationTime).ToUnixTimeSeconds()
            };

            var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
            var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson)).Replace("+", "-").Replace("/", "_").TrimEnd('=');

            // Create signature
            var messageBytes = Encoding.UTF8.GetBytes($"{headerBase64}.{payloadBase64}");
            byte[] keyBytes = Encoding.UTF8.GetBytes(_secretKey);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(messageBytes);
                var signatureBase64 = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');

                // Combine all parts
                var token = $"{headerBase64}.{payloadBase64}.{signatureBase64}";
                return token;
            }
        }

        /// <summary>
        /// Extracts the TenantId from a JWT token without validation.
        /// Used to determine which tenant's data to return.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>The TenantId from the token.</returns>
        public string GetTenantIdFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3) return null;

                var payload = parts[1];
                // Add padding if needed
                var padding = 4 - (payload.Length % 4);
                if (padding != 4) payload += new string('=', padding);

                var decodedBytes = Convert.FromBase64String(payload.Replace("-", "+").Replace("_", "/"));
                var decodedString = Encoding.UTF8.GetString(decodedBytes);

                var doc = System.Text.Json.JsonDocument.Parse(decodedString);
                if (doc.RootElement.TryGetProperty("TenantId", out var tenantIdElement))
                {
                    return tenantIdElement.GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts the user ID from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>The user ID from the token.</returns>
        public string GetUserIdFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3) return null;

                var payload = parts[1];
                // Add padding if needed
                var padding = 4 - (payload.Length % 4);
                if (padding != 4) payload += new string('=', padding);

                var decodedBytes = Convert.FromBase64String(payload.Replace("-", "+").Replace("_", "/"));
                var decodedString = Encoding.UTF8.GetString(decodedBytes);

                var doc = System.Text.Json.JsonDocument.Parse(decodedString);
                if (doc.RootElement.TryGetProperty("nameid", out var userIdElement))
                {
                    return userIdElement.GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}


