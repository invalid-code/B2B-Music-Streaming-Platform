namespace API.Models.DTOs.Response.Auth
{
    /// <summary>
    /// Response model for successful authentication (login/register).
    /// Contains the JWT token that encapsulates the TenantId.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT token for authenticating subsequent requests.
        /// Contains the TenantId in the payload.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// When the token expires (UTC).
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// User information.
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// Tenant (Venue) information.
        /// </summary>
        public TenantInfo Tenant { get; set; }
    }

    /// <summary>
    /// User information in the auth response.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// User's role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Tenant ID (Venue ID) this user belongs to.
        /// </summary>
        public string TenantId { get; set; }
    }

    /// <summary>
    /// Tenant (Venue) information in the auth response.
    /// </summary>
    public class TenantInfo
    {
        /// <summary>
        /// Tenant ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tenant name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Subscription plan type.
        /// </summary>
        public string PlanType { get; set; }

        /// <summary>
        /// Is this a trial tenant?
        /// </summary>
        public bool IsTrial => PlanType == "Trial";

        /// <summary>
        /// Remaining trial time in seconds (if trial).
        /// </summary>
        public int? RemainingTrialSeconds { get; set; }
    }
}
