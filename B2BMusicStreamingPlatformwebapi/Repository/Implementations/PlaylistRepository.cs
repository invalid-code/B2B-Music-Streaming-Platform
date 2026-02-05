using API.Data;
using API.Models.Core_Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implementations
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly MusicStreamingDbContext _dbContext;

        public PlaylistRepository(MusicStreamingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Playlist> GetByIdAsync(string id)
        {
            return await _dbContext.Playlists.FirstOrDefaultAsync(p => p.PlaylistID == id);
        }

        public async Task<List<Playlist>> GetAllAsync()
        {
            return await _dbContext.Playlists.ToListAsync();
        }

        public async Task<Playlist> AddAsync(Playlist entity)
        {
            await _dbContext.Playlists.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Playlist entity)
        {
            var existingPlaylist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.PlaylistID == entity.PlaylistID);
            if (existingPlaylist == null)
                return false;

            existingPlaylist.Name = entity.Name;
            existingPlaylist.VibeOrGenre = entity.VibeOrGenre;
            existingPlaylist.TrackIDs = entity.TrackIDs;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var playlist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.PlaylistID == id);
            if (playlist == null)
                return false;

            _dbContext.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Playlists.AnyAsync(p => p.PlaylistID == id);
        }

        public async Task<List<Playlist>> GetPlaylistsByGenreAsync(string genre)
        {
            return await _dbContext.Playlists
                .Where(p => p.VibeOrGenre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<bool> AddTrackToPlaylistAsync(string playlistId, string trackId)
        {
            var playlist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.PlaylistID == playlistId);
            if (playlist == null)
                return false;

            if (playlist.TrackIDs == null)
                playlist.TrackIDs = new List<string>();

            if (!playlist.TrackIDs.Contains(trackId))
                playlist.TrackIDs.Add(trackId);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTrackFromPlaylistAsync(string playlistId, string trackId)
        {
            var playlist = await _dbContext.Playlists.FirstOrDefaultAsync(p => p.PlaylistID == playlistId);
            if (playlist == null)
                return false;

            if (playlist.TrackIDs != null)
                playlist.TrackIDs.Remove(trackId);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}