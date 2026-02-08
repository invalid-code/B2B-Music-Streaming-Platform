using API.Interface;
using API.Models.Core_Models;
using API.Models.DTOs.Requests.Track;
using API.Models.DTOs.Response.track;
using API.Repository;

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
