namespace API.Models.Entities
{
    /// <summary>
    /// Represents a Venue (Business Client) in the multi-tenant system.
    /// A Tenant is a venue that streams music to its customers.
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Unique identifier for the tenant.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the venue.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Physical location of the venue.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Stripe customer ID for billing integration.
        /// </summary>
        public string StripeCustomerId { get; set; }

        /// <summary>
        /// Subscription plan type: "Trial" or "Paid".
        /// Trial: 30-minute streaming limit.
        /// Paid: Unlimited streaming.
        /// </summary>
        public string PlanType { get; set; } = "Trial"; // "Trial" or "Paid"

        /// <summary>
        /// When the trial started (null if not applicable).
        /// </summary>
        public DateTime? TrialStartDate { get; set; }

        /// <summary>
        /// When the subscription becomes active (for paid plans).
        /// </summary>
        public DateTime? SubscriptionStartDate { get; set; }

        /// <summary>
        /// Navigation property: Users belonging to this tenant.
        /// </summary>
        public virtual ICollection<API.Models.Identity.ApplicationUser> Users { get; set; } = new List<API.Models.Identity.ApplicationUser>();

        /// <summary>
        /// When this tenant was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this tenant was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Is the tenant active?
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
