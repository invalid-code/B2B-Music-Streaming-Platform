using API.Models.Core_Models;

namespace API.Repository
{
    public interface IPlaylistRepository : IGenericRepository<Playlist>
    {
        Task<List<Playlist>> GetPlaylistsByGenreAsync(string genre);
        Task<bool> AddTrackToPlaylistAsync(string playlistId, string trackId);
        Task<bool> RemoveTrackFromPlaylistAsync(string playlistId, string trackId);
    }
}
