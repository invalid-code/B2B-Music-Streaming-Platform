using API.Models.Entities;

namespace API.Repository
{
    public interface IVenueRepository : IGenericRepository<Venue>
    {
        Task<List<Venue>> GetVenuesBySubscriptionStatusAsync(string status);
        Task<List<Venue>> GetVenuesByLocationAsync(string location);
    }
}
