using API.Models.DTOs.Requests.Venue;
using API.Models.DTOs.Response.venue;

namespace API.Interface
{
    public interface IVenueService
    {
        Task<List<VenueResponse>> GetAllVenuesAsync();
        Task<VenueResponse> GetVenueByIdAsync(string id);
        Task<VenueResponse> CreateVenueAsync(CreateVenueRequest request);
        Task<bool> UpdateVenueAsync(string id, UpdateVenueRequest request);
        Task<bool> DeleteVenueAsync(string id);
    }
}
