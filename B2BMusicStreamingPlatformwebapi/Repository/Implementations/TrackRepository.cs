using API.Models.Core_Models;

namespace API.Repository.Implementations
{
    public class TrackRepository : GenericRepository<Track>, ITrackRepository
    {
        public TrackRepository() : base()
        {
        }

        public override Task<Track> GetByIdAsync(string id)
        {
            var track = GetData().FirstOrDefault(t => t.TrackID == id);
            return Task.FromResult(track);
        }

        public override Task<bool> UpdateAsync(Track entity)
        {
            var existingTrack = GetData().FirstOrDefault(t => t.TrackID == entity.TrackID);
            if (existingTrack == null)
                return Task.FromResult(false);

            existingTrack.Title = entity.Title;
            existingTrack.Artist = entity.Artist;
            existingTrack.Mood = entity.Mood;
            existingTrack.CloudflareStorageKey = entity.CloudflareStorageKey;

            return Task.FromResult(true);
        }

        public override Task<bool> DeleteAsync(string id)
        {
            var track = GetData().FirstOrDefault(t => t.TrackID == id);
            if (track == null)
                return Task.FromResult(false);

            GetData().Remove(track);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(string id)
        {
            return Task.FromResult(GetData().Any(t => t.TrackID == id));
        }

        public Task<List<Track>> GetTracksByMoodAsync(string mood)
        {
            var tracks = GetData()
                .Where(t => t.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(tracks);
        }

        public Task<List<Track>> GetTracksByArtistAsync(string artist)
        {
            var tracks = GetData()
                .Where(t => t.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(tracks);
        }
    }
}
