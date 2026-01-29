using API.Interface;
using API.Models.Core_Models;
using API.Models.DTOs.Requests.Playlist;
using PlaylistResponseDto = API.Models.DTOs.Response.playlist.PlaylistResponse;
using API.Repository;

namespace API.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;

        public PlaylistService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public async Task<List<PlaylistResponseDto>> GetAllPlaylistsAsync()
        {
            var playlists = await _playlistRepository.GetAllAsync();
            return playlists.Select(MapToResponse).ToList();
        }

        public async Task<PlaylistResponseDto> GetPlaylistByIdAsync(string id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null)
                return null;
            return MapToResponse(playlist);
        }

        public async Task<PlaylistResponseDto> CreatePlaylistAsync(CreatePlaylistRequest request)
        {
            var playlist = new Playlist
            {
                PlaylistID = Guid.NewGuid().ToString(),
                Name = request.Name,
                VibeOrGenre = request.VibeOrGenre,
                TrackIDs = request.TrackIDs ?? new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            var createdPlaylist = await _playlistRepository.AddAsync(playlist);
            return MapToResponse(createdPlaylist);
        }

        public async Task<bool> UpdatePlaylistAsync(string id, UpdatePlaylistRequest request)
        {
            var existingPlaylist = await _playlistRepository.GetByIdAsync(id);
            if (existingPlaylist == null)
                return false;

            existingPlaylist.Name = request.Name ?? existingPlaylist.Name;
            existingPlaylist.VibeOrGenre = request.VibeOrGenre ?? existingPlaylist.VibeOrGenre;
            existingPlaylist.TrackIDs = request.TrackIDs ?? existingPlaylist.TrackIDs;

            return await _playlistRepository.UpdateAsync(existingPlaylist);
        }

        public async Task<bool> DeletePlaylistAsync(string id)
        {
            return await _playlistRepository.DeleteAsync(id);
        }

        private PlaylistResponseDto MapToResponse(Playlist playlist)
        {
            return new PlaylistResponseDto
            {
                PlaylistID = playlist.PlaylistID,
                Name = playlist.Name,
                VibeOrGenre = playlist.VibeOrGenre,
                TrackIDs = playlist.TrackIDs
            };
        }
    }
}
