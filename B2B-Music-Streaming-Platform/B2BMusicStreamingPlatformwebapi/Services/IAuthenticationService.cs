using API.Models.DTOs.Requests.Auth;
using API.Models.DTOs.Response.Auth;

namespace API.Services
{
    /// <summary>
    /// Interface for authentication service.
    /// Handles user registration and login.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registers a new venue owner.
        /// Creates both a Tenant (venue) and User (owner) record.
        /// </summary>
        /// <param name="request">Registration request.</param>
        /// <returns>Tuple with success flag, auth response, and error message.</returns>
        Task<(bool Success, AuthResponse Response, string Error)> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// The token contains the TenantId for multi-tenant isolation.
        /// </summary>
        /// <param name="request">Login request.</param>
        /// <returns>Tuple with success flag, auth response, and error message.</returns>
        Task<(bool Success, AuthResponse Response, string Error)> LoginAsync(LoginRequest request);

        /// <summary>
        /// Logs out a user.
        /// </summary>
        Task LogoutAsync();
    }
}
