using API.Models.Core_Models;

namespace API.Repository.Implementations
{
    public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository() : base()
        {
        }

        public override Task<Playlist> GetByIdAsync(string id)
        {
            var playlist = GetData().FirstOrDefault(p => p.PlaylistID == id);
            return Task.FromResult(playlist);
        }

        public override Task<bool> UpdateAsync(Playlist entity)
        {
            var existingPlaylist = GetData().FirstOrDefault(p => p.PlaylistID == entity.PlaylistID);
            if (existingPlaylist == null)
                return Task.FromResult(false);

            existingPlaylist.Name = entity.Name;
            existingPlaylist.VibeOrGenre = entity.VibeOrGenre;
            existingPlaylist.TrackIDs = entity.TrackIDs;

            return Task.FromResult(true);
        }

        public override Task<bool> DeleteAsync(string id)
        {
            var playlist = GetData().FirstOrDefault(p => p.PlaylistID == id);
            if (playlist == null)
                return Task.FromResult(false);

            GetData().Remove(playlist);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(string id)
        {
            return Task.FromResult(GetData().Any(p => p.PlaylistID == id));
        }

        public Task<List<Playlist>> GetPlaylistsByGenreAsync(string genre)
        {
            var playlists = GetData()
                .Where(p => p.VibeOrGenre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(playlists);
        }

        public Task<bool> AddTrackToPlaylistAsync(string playlistId, string trackId)
        {
            var playlist = GetData().FirstOrDefault(p => p.PlaylistID == playlistId);
            if (playlist == null)
                return Task.FromResult(false);

            if (playlist.TrackIDs == null)
                playlist.TrackIDs = new List<string>();

            if (!playlist.TrackIDs.Contains(trackId))
                playlist.TrackIDs.Add(trackId);

            return Task.FromResult(true);
        }

        public Task<bool> RemoveTrackFromPlaylistAsync(string playlistId, string trackId)
        {
            var playlist = GetData().FirstOrDefault(p => p.PlaylistID == playlistId);
            if (playlist == null)
                return Task.FromResult(false);

            if (playlist.TrackIDs != null)
                playlist.TrackIDs.Remove(trackId);

            return Task.FromResult(true);
        }
    }
}
