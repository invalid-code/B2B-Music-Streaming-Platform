using API.Models.Entities;

namespace API.Repository
{
    /// <summary>
    /// Repository interface for Tenant (Venue) operations.
    /// </summary>
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
        /// <summary>
        /// Get a tenant by name.
        /// </summary>
        Task<Tenant> GetByNameAsync(string name);

        /// <summary>
        /// Get all active tenants.
        /// </summary>
        Task<List<Tenant>> GetActiveTenantAsync();

        /// <summary>
        /// Get tenants by subscription plan type.
        /// </summary>
        Task<List<Tenant>> GetByPlanTypeAsync(string planType);

        /// <summary>
        /// Get trial tenants that have exceeded the 30-minute limit.
        /// </summary>
        Task<List<Tenant>> GetExpiredTrialsAsync();
    }
}
