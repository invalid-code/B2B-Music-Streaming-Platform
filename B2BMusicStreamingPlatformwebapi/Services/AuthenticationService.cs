using API.Models.DTOs.Requests.Auth;
using API.Models.DTOs.Response.Auth;
using API.Models.Entities;
using API.Models.Identity;
using API.Data;

namespace API.Services
{
    /// <summary>
    /// Service for handling user registration and authentication.
    /// Implements the multi-tenant pattern where each registration creates a Tenant.
    /// Uses Entity Framework Core with PostgreSQL for data persistence.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly MusicStreamingDbContext _dbContext;

        public AuthenticationService(IJwtTokenService jwtTokenService, MusicStreamingDbContext dbContext)
        {
            _jwtTokenService = jwtTokenService;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Registers a new venue owner and creates an associated tenant.
        /// </summary>
        /// <param name="request">Registration request with venue and user details.</param>
        /// <returns>Authentication response with JWT token.</returns>
        public async Task<(bool Success, AuthResponse Response, string Error)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if email already exists
                if (_dbContext.Users.Any(u => u.Email == request.Email))
                {
                    return (false, null, "Email already registered");
                }

                // Create tenant first (the venue)
                var tenant = new Tenant
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.VenueName,
                    Location = request.Location,
                    PlanType = "Trial",
                    TrialStartDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // _tenants.Add(tenant);
                await _dbContext.Tenants.AddAsync(tenant);
                await _dbContext.SaveChangesAsync();

                // Create user linked to tenant
                ApplicationUser user;
                if (request.Role == "BusinessOwner")
                {
                    user = new BusinessOwner
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = request.Email,
                        FullName = request.FullName,
                        PasswordHash = HashPassword(request.Password),
                        TenantId = tenant.Id,
                        Role = request.Role,
                        CreatedAt = DateTime.UtcNow,
                        BusinessRegistrationNumber = request.BusinessRegistrationNumber
                    };
                }
                else
                {
                    user = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = request.Email,
                        FullName = request.FullName,
                        PasswordHash = HashPassword(request.Password),
                        TenantId = tenant.Id,
                        Role = request.Role,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                // _users.Add(user);
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(user, tenant.Id);

                var response = new AuthResponse
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        TenantId = tenant.Id
                    },
                    Tenant = new TenantInfo
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        PlanType = tenant.PlanType,
                        RemainingTrialSeconds = 1800
                    }
                };

                return (true, response, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>Authentication response with JWT token.</returns>
        public async Task<(bool Success, AuthResponse Response, string Error)> LoginAsync(LoginRequest request)
        {
            try
            {
                // Find user by email
                // var user = _users.FirstOrDefault(u => u.Email == request.Email);
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    return (false, null, "Invalid email or password");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return (false, null, "User account is inactive");
                }

                // Get tenant information
                // var tenant = _tenants.FirstOrDefault(t => t.Id == user.TenantId);
                var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Id == user.TenantId);
                if (tenant == null)
                {
                    return (false, null, "Tenant not found");
                }

                // Calculate remaining trial time
                int? remainingTrialSeconds = null;
                if (tenant.PlanType == "Trial" && tenant.TrialStartDate.HasValue)
                {
                    var elapsed = (DateTime.UtcNow - tenant.TrialStartDate.Value).TotalSeconds;
                    remainingTrialSeconds = Math.Max(0, (int)(1800 - elapsed));
                }

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(user, user.TenantId);

                var response = new AuthResponse
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        TenantId = user.TenantId
                    },
                    Tenant = new TenantInfo
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        PlanType = tenant.PlanType,
                        RemainingTrialSeconds = remainingTrialSeconds
                    }
                };

                return (true, response, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Logs out a user (client should discard JWT token).
        /// </summary>
        public async Task LogoutAsync()
        {
            await Task.CompletedTask;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(password)
                )
            );
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}
