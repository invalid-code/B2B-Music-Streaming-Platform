using API.Models.DTOs.Requests.Auth;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Authentication controller for registration and login.
    /// Implements the "Zero-Install" Auth Flow:
    /// 1. Registration: Create Tenant (Venue) and User
    /// 2. Login: Return JWT with TenantId encoded
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new venue owner.
        /// This creates:
        /// 1. A new Tenant (Venue) record
        /// 2. A new User (Owner) record linked to the Tenant
        /// </summary>
        /// <param name="request">Registration details.</param>
        /// <returns>JWT token and user/tenant information.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required");
            }

            if (string.IsNullOrWhiteSpace(request.VenueName) || string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest("Venue name and location are required");
            }

            _logger.LogInformation($"Registration attempt for email: {request.Email}, venue: {request.VenueName}");

            // Call authentication service
            var (success, response, error) = await _authService.RegisterAsync(request);

            if (!success)
            {
                _logger.LogWarning($"Registration failed for {request.Email}: {error}");
                return BadRequest(new { message = error });
            }

            _logger.LogInformation($"Registration successful for {request.Email}");

            // Return 201 Created with the authentication response
            return CreatedAtAction(nameof(Register), response);
        }

        /// <summary>
        /// Login with email and password.
        /// Returns a JWT token that contains:
        /// - User ID
        /// - Email
        /// - Role
        /// - TenantId (for multi-tenant isolation)
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>JWT token and user/tenant information.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required");
            }

            _logger.LogInformation($"Login attempt for email: {request.Email}");

            // Call authentication service
            var (success, response, error) = await _authService.LoginAsync(request);

            if (!success)
            {
                _logger.LogWarning($"Login failed for {request.Email}: {error}");
                return Unauthorized(new { message = error });
            }

            _logger.LogInformation($"Login successful for {request.Email}");

            // Return 200 OK with the authentication response
            return Ok(response);
        }

        /// <summary>
        /// Logout endpoint (client should discard JWT token).
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Logout successful" });
        }
    }
}
