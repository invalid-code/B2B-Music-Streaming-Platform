using API.Interface;
using API.Models.DTOs.Requests.Venue;
using API.Models.DTOs.Response.venue;
using API.Models.Entities;
using API.Repository;

namespace API.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;

        public VenueService(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<List<VenueResponse>> GetAllVenuesAsync()
        {
            var venues = await _venueRepository.GetAllAsync();
            return venues.Select(MapToResponse).ToList();
        }

        public async Task<VenueResponse> GetVenueByIdAsync(string id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
                return null;
            return MapToResponse(venue);
        }

        public async Task<VenueResponse> CreateVenueAsync(CreateVenueRequest request)
        {
            var venue = CreateVenueInstance(request.SubscriptionStatus);
            venue.VenueID = Guid.NewGuid().ToString();
            venue.BusinessName = request.BusinessName;
            venue.Location = request.Location;
            venue.SubscriptionStatus = request.SubscriptionStatus;

            var createdVenue = await _venueRepository.AddAsync(venue);
            return MapToResponse(createdVenue);
        }

        public async Task<bool> UpdateVenueAsync(string id, UpdateVenueRequest request)
        {
            var existingVenue = await _venueRepository.GetByIdAsync(id);
            if (existingVenue == null)
                return false;

            existingVenue.BusinessName = request.BusinessName ?? existingVenue.BusinessName;
            existingVenue.Location = request.Location ?? existingVenue.Location;
            existingVenue.SubscriptionStatus = request.SubscriptionStatus ?? existingVenue.SubscriptionStatus;

            return await _venueRepository.UpdateAsync(existingVenue);
        }

        public async Task<bool> DeleteVenueAsync(string id)
        {
            return await _venueRepository.DeleteAsync(id);
        }

        private Venue CreateVenueInstance(string subscriptionStatus)
        {
            return subscriptionStatus?.ToLower() switch
            {
                "trial" => new TrialVenue(),
                "paid" => new PaidVenue(),
                _ => throw new ArgumentException($"Invalid subscription status: {subscriptionStatus}")
            };
        }

        private VenueResponse MapToResponse(Venue venue)
        {
            return new VenueResponse
            {
                VenueID = venue.VenueID,
                BusinessName = venue.BusinessName,
                Location = venue.Location,
                SubscriptionStatus = venue.SubscriptionStatus
            };
        }
    }
}
