using API.Models.Entities;

namespace API.Repository.Implementations
{
    /// <summary>
    /// Repository implementation for Tenant (Venue) operations.
    /// </summary>
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository() : base()
        {
        }

        public override Task<Tenant> GetByIdAsync(string id)
        {
            var tenant = GetData().FirstOrDefault(t => t.Id == id);
            return Task.FromResult(tenant);
        }

        public override Task<bool> UpdateAsync(Tenant entity)
        {
            var existingTenant = GetData().FirstOrDefault(t => t.Id == entity.Id);
            if (existingTenant == null)
                return Task.FromResult(false);

            existingTenant.Name = entity.Name;
            existingTenant.Location = entity.Location;
            existingTenant.PlanType = entity.PlanType;
            existingTenant.StripeCustomerId = entity.StripeCustomerId;
            existingTenant.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult(true);
        }

        public override Task<bool> DeleteAsync(string id)
        {
            var tenant = GetData().FirstOrDefault(t => t.Id == id);
            if (tenant == null)
                return Task.FromResult(false);

            GetData().Remove(tenant);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(string id)
        {
            return Task.FromResult(GetData().Any(t => t.Id == id));
        }

        public Task<Tenant> GetByNameAsync(string name)
        {
            var tenant = GetData().FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(tenant);
        }

        public Task<List<Tenant>> GetActiveTenantAsync()
        {
            var tenants = GetData().Where(t => t.IsActive).ToList();
            return Task.FromResult(tenants);
        }

        public Task<List<Tenant>> GetByPlanTypeAsync(string planType)
        {
            var tenants = GetData()
                .Where(t => t.PlanType.Equals(planType, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(tenants);
        }

        public Task<List<Tenant>> GetExpiredTrialsAsync()
        {
            var now = DateTime.UtcNow;
            var tenants = GetData()
                .Where(t => t.PlanType == "Trial" && t.TrialStartDate.HasValue &&
                       (now - t.TrialStartDate.Value).TotalSeconds > 1800) // 30 minutes
                .ToList();
            return Task.FromResult(tenants);
        }
    }
}
