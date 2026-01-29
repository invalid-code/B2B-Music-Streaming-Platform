using API.Models.Entities;

namespace API.Repository.Implementations
{
    public class VenueRepository : GenericRepository<Venue>, IVenueRepository
    {
        public VenueRepository() : base()
        {
        }

        public override Task<Venue> GetByIdAsync(string id)
        {
            var venue = GetData().FirstOrDefault(v => v.VenueID == id);
            return Task.FromResult(venue);
        }

        public override Task<bool> UpdateAsync(Venue entity)
        {
            var existingVenue = GetData().FirstOrDefault(v => v.VenueID == entity.VenueID);
            if (existingVenue == null)
                return Task.FromResult(false);

            existingVenue.BusinessName = entity.BusinessName;
            existingVenue.Location = entity.Location;
            existingVenue.SubscriptionStatus = entity.SubscriptionStatus;

            return Task.FromResult(true);
        }

        public override Task<bool> DeleteAsync(string id)
        {
            var venue = GetData().FirstOrDefault(v => v.VenueID == id);
            if (venue == null)
                return Task.FromResult(false);

            GetData().Remove(venue);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(string id)
        {
            return Task.FromResult(GetData().Any(v => v.VenueID == id));
        }

        public Task<List<Venue>> GetVenuesBySubscriptionStatusAsync(string status)
        {
            var venues = GetData()
                .Where(v => v.SubscriptionStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(venues);
        }

        public Task<List<Venue>> GetVenuesByLocationAsync(string location)
        {
            var venues = GetData()
                .Where(v => v.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(venues);
        }
    }
}
