using API.Models.Core_Models;

namespace API.Repository
{
    public interface ITrackRepository : IGenericRepository<Track>
    {
        Task<List<Track>> GetTracksByMoodAsync(string mood);
        Task<List<Track>> GetTracksByArtistAsync(string artist);
    }
}
