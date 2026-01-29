using API.Models.DTOs.Requests.Track;
using API.Models.DTOs.Response.track;

namespace API.Interface
{
    public interface ITrackService
    {
        Task<List<TrackResponse>> GetAllTracksAsync();
        Task<TrackResponse> GetTrackByIdAsync(string id);
        Task<TrackResponse> CreateTrackAsync(CreateTrackRequest request);
        Task<bool> UpdateTrackAsync(string id, UpdateTrackRequest request);
        Task<bool> DeleteTrackAsync(string id);
    }
}
