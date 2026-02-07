using API.Models.DTOs.Requests.Track;
using API.Models.DTOs.Response.track;
using API.Services;

namespace API.Interface
{
    public interface ITrackService
    {
        Task<List<TrackResponse>> GetAllTracksAsync();
        Task<TrackResponse> GetTrackByIdAsync(string id);
        Task<TrackResponse> CreateTrackAsync(CreateTrackRequest request);
        Task<bool> UpdateTrackAsync(string id, UpdateTrackRequest request);
        Task<bool> DeleteTrackAsync(string id);

        /// <summary>
        /// Uploads a new track with audio file to Cloudflare R2 and creates the track record.
        /// </summary>
        Task<UploadTrackResponse> UploadTrackAsync(
            UploadTrackRequest request,
            ICloudflareR2Service r2Service,
            Microsoft.Extensions.Logging.ILogger<TrackService> logger);

        /// <summary>
        /// Generates a signed URL for secure audio streaming.
        /// </summary>
        Task<StreamAuthorizationResponse> AuthorizeStreamAsync(
            StreamAuthorizationRequest request,
            ICloudflareR2Service r2Service,
            Microsoft.Extensions.Logging.ILogger<TrackService> logger);
    }
}
