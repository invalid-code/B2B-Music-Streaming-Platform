using API.Data;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implementations
{
    /// <summary>
    /// Repository implementation for Tenant (Venue) operations.
    /// </summary>
    public class TenantRepository : ITenantRepository
    {
        private readonly MusicStreamingDbContext _dbContext;

        public TenantRepository(MusicStreamingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tenant> GetByIdAsync(string id)
        {
            return await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tenant>> GetAllAsync()
        {
            return await _dbContext.Tenants.ToListAsync();
        }

        public async Task<Tenant> AddAsync(Tenant entity)
        {
            await _dbContext.Tenants.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Tenant entity)
        {
            var existingTenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (existingTenant == null)
                return false;

            existingTenant.Name = entity.Name;
            existingTenant.Location = entity.Location;
            existingTenant.PlanType = entity.PlanType;
            existingTenant.StripeCustomerId = entity.StripeCustomerId;
            existingTenant.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
                return false;

            _dbContext.Tenants.Remove(tenant);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Tenants.AnyAsync(t => t.Id == id);
        }

        public async Task<Tenant> GetByNameAsync(string name)
        {
            return await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<Tenant>> GetActiveTenantAsync()
        {
            return await _dbContext.Tenants
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<List<Tenant>> GetByPlanTypeAsync(string planType)
        {
            return await _dbContext.Tenants
                .Where(t => t.PlanType.Equals(planType, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<List<Tenant>> GetExpiredTrialsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbContext.Tenants
                .Where(t => t.PlanType == "Trial" && t.TrialStartDate.HasValue &&
                       (now - t.TrialStartDate.Value).TotalSeconds > 1800) // 30 minutes
                .ToListAsync();
        }
    }
}