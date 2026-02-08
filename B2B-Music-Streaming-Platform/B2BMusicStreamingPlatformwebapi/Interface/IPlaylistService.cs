using API.Models.DTOs.Requests.Playlist;
using PlaylistResponseDto = API.Models.DTOs.Response.playlist.PlaylistResponse;

namespace API.Interface
{
    public interface IPlaylistService
    {
        Task<List<PlaylistResponseDto>> GetAllPlaylistsAsync();
        Task<PlaylistResponseDto> GetPlaylistByIdAsync(string id);
        Task<PlaylistResponseDto> CreatePlaylistAsync(CreatePlaylistRequest request);
        Task<bool> UpdatePlaylistAsync(string id, UpdatePlaylistRequest request);
        Task<bool> DeletePlaylistAsync(string id);
    }
}
