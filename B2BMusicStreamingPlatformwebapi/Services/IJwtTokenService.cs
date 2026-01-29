using API.Models.Identity;

namespace API.Services
{
    /// <summary>
    /// Interface for JWT token service.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT token for an authenticated user.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <param name="tenantId">The tenant ID.</param>
        /// <returns>JWT token string.</returns>
        string GenerateToken(ApplicationUser user, string tenantId);

        /// <summary>
        /// Extracts the TenantId from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>The TenantId.</returns>
        string GetTenantIdFromToken(string token);

        /// <summary>
        /// Extracts the user ID from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>The user ID.</returns>
        string GetUserIdFromToken(string token);
    }
}
