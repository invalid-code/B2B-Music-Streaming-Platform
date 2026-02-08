using API.Data;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implementations
{
    public class VenueRepository : IVenueRepository
    {
        private readonly MusicStreamingDbContext _dbContext;

        public VenueRepository(MusicStreamingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Venue> GetByIdAsync(string id)
        {
            return await _dbContext.Venues.FirstOrDefaultAsync(v => v.VenueID == id);
        }

        public async Task<List<Venue>> GetAllAsync()
        {
            return await _dbContext.Venues.ToListAsync();
        }

        public async Task<Venue> AddAsync(Venue entity)
        {
            await _dbContext.Venues.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Venue entity)
        {
            var existingVenue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.VenueID == entity.VenueID);
            if (existingVenue == null)
                return false;

            existingVenue.BusinessName = entity.BusinessName;
            existingVenue.Location = entity.Location;
            existingVenue.SubscriptionStatus = entity.SubscriptionStatus;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.VenueID == id);
            if (venue == null)
                return false;

            _dbContext.Venues.Remove(venue);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Venues.AnyAsync(v => v.VenueID == id);
        }

        public async Task<List<Venue>> GetVenuesBySubscriptionStatusAsync(string status)
        {
            return await _dbContext.Venues
                .Where(v => v.SubscriptionStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<List<Venue>> GetVenuesByLocationAsync(string location)
        {
            return await _dbContext.Venues
                .Where(v => v.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
    }
}