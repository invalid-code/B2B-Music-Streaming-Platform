using API.Interface;
using API.Models.Core_Models;
using API.Models.DTOs.Requests.Track;
using API.Models.DTOs.Response.track;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class TrackService : ITrackService
    {
        private readonly ITrackRepository _trackRepository;

        public TrackService(ITrackRepository trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<List<TrackResponse>> GetAllTracksAsync()
        {
            var tracks = await _trackRepository.GetAllAsync();
            return tracks.Select(MapToResponse).ToList();
        }

        public async Task<TrackResponse> GetTrackByIdAsync(string id)
        {
            var track = await _trackRepository.GetByIdAsync(id);
            if (track == null)
                return null;
            return MapToResponse(track);
        }

        public async Task<TrackResponse> CreateTrackAsync(CreateTrackRequest request)
        {
            var track = new Track
            {
                TrackID = Guid.NewGuid().ToString(),
                Title = request.Title,
                Artist = request.Artist,
                Mood = request.Mood,
                CloudflareStorageKey = request.CloudflareStorageKey,
                UploadedAt = DateTime.UtcNow
            };

            var createdTrack = await _trackRepository.AddAsync(track);
            return MapToResponse(createdTrack);
        }

        public async Task<bool> UpdateTrackAsync(string id, UpdateTrackRequest request)
        {
            var existingTrack = await _trackRepository.GetByIdAsync(id);
            if (existingTrack == null)
                return false;

            existingTrack.Title = request.Title ?? existingTrack.Title;
            existingTrack.Artist = request.Artist ?? existingTrack.Artist;
            existingTrack.Mood = request.Mood ?? existingTrack.Mood;
            existingTrack.CloudflareStorageKey = request.CloudflareStorageKey ?? existingTrack.CloudflareStorageKey;

            return await _trackRepository.UpdateAsync(existingTrack);
        }

        public async Task<bool> DeleteTrackAsync(string id)
        {
            return await _trackRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Uploads a new track with audio file to Cloudflare R2 and creates the track record.
        /// </summary>
        /// <param name="request">The upload request containing track metadata and audio file</param>
        /// <param name="r2Service">The Cloudflare R2 service for file upload</param>
        /// <param name="logger">Logger for tracking operations</param>
        /// <returns>The upload response with track details</returns>
        public async Task<UploadTrackResponse> UploadTrackAsync(
            UploadTrackRequest request,
            ICloudflareR2Service r2Service,
            ILogger<TrackService> logger)
        {
            try
            {
                // Validate the audio file
                if (!r2Service.ValidateAudioFile(request.AudioFile.OpenReadStream(), request.AudioFile.FileName))
                {
                    return new UploadTrackResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid audio file. Please ensure the file is a supported format (MP3, WAV, FLAC, M4A, AAC) and under 100MB."
                    };
                }

                // Upload the audio file to Cloudflare R2
                var storageKey = await r2Service.UploadAudioFileAsync(
                    request.AudioFile.OpenReadStream(),
                    request.AudioFile.FileName,
                    request.AudioFile.ContentType);

                // Create the track record in the database
                var track = new Track
                {
                    TrackID = Guid.NewGuid().ToString(),
                    Title = request.Title,
                    Artist = request.Artist,
                    Mood = request.Mood,
                    CloudflareStorageKey = storageKey,
                    UploadedAt = DateTime.UtcNow
                };

                var createdTrack = await _trackRepository.AddAsync(track);

                logger.LogInformation($"Successfully uploaded track '{createdTrack.Title}' by {createdTrack.Artist} with storage key: {storageKey}");

                return new UploadTrackResponse
                {
                    TrackID = createdTrack.TrackID,
                    Title = createdTrack.Title,
                    Artist = createdTrack.Artist,
                    Mood = createdTrack.Mood,
                    CloudflareStorageKey = createdTrack.CloudflareStorageKey,
                    UploadedAt = createdTrack.UploadedAt,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading track");
                return new UploadTrackResponse
                {
                    Success = false,
                    ErrorMessage = $"Failed to upload track: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Generates a signed URL for secure audio streaming.
        /// </summary>
        /// <param name="request">The stream authorization request</param>
        /// <param name="r2Service">The Cloudflare R2 service for signed URL generation</param>
        /// <param name="logger">Logger for tracking operations</param>
        /// <returns>The stream authorization response with signed URL</returns>
        public async Task<StreamAuthorizationResponse> AuthorizeStreamAsync(
            StreamAuthorizationRequest request,
            ICloudflareR2Service r2Service,
            ILogger<TrackService> logger)
        {
            try
            {
                // Validate that the track exists
                var track = await _trackRepository.GetByIdAsync(request.TrackId);
                if (track == null)
                {
                    return new StreamAuthorizationResponse
                    {
                        Success = false,
                        ErrorMessage = "Track not found"
                    };
                }

                // Generate the signed URL
                var signedUrl = await r2Service.GenerateSignedUrlAsync(
                    track.CloudflareStorageKey,
                    request.VenueId,
                    request.TrackId);

                var authorizedDuration = request.PlaybackDuration ?? 900; // Default 15 minutes

                logger.LogInformation($"Generated signed URL for track '{track.Title}' (ID: {request.TrackId}) for venue {request.VenueId}");

                return new StreamAuthorizationResponse
                {
                    TrackId = request.TrackId,
                    VenueId = request.VenueId,
                    SignedUrl = signedUrl,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    AuthorizedDuration = authorizedDuration,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error authorizing stream for track {request.TrackId} and venue {request.VenueId}");
                return new StreamAuthorizationResponse
                {
                    Success = false,
                    ErrorMessage = $"Failed to authorize stream: {ex.Message}"
                };
            }
        }

        private TrackResponse MapToResponse(Track track)
        {
            return new TrackResponse
            {
                TrackID = track.TrackID,
                Title = track.Title,
                Artist = track.Artist,
                Mood = track.Mood,
                CloudflareStorageKey = track.CloudflareStorageKey
            };
        }
    }
}
