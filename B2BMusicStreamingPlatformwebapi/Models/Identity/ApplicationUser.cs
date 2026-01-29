namespace API.Models.Identity
{
    /// <summary>
    /// Base user class for authentication.
    /// Contains common properties for all user types.
    /// Will be extended with ASP.NET Core Identity when database is added.
    /// </summary>
    public class ApplicationUser
    {
        /// <summary>
        /// Unique user identifier.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// User's email address (username for login).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Hashed password (will use ASP.NET Core Identity when database is added).
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Foreign key to the Tenant (Venue) this user belongs to.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// User's role: BusinessOwner, SystemAdmin, or Staff
        /// </summary>
        public string Role { get; set; } = "BusinessOwner"; // "BusinessOwner", "SystemAdmin", "Staff"

        /// <summary>
        /// When the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Track if user is active/inactive.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// BusinessOwner role: Manages a venue profile and billing.
    /// </summary>
    public class BusinessOwner : ApplicationUser
    {
        /// <summary>
        /// Business registration number or tax ID.
        /// </summary>
        public string BusinessRegistrationNumber { get; set; }

        /// <summary>
        /// Business license expiry date.
        /// </summary>
        public DateTime? LicenseExpiryDate { get; set; }
    }

    /// <summary>
    /// SystemAdmin role: Uploads music and curates playlists.
    /// </summary>
    public class SystemAdmin : ApplicationUser
    {
        /// <summary>
        /// Admin-specific permissions (for future RBAC extensions).
        /// </summary>
        public List<string> Permissions { get; set; } = new();
    }
}

