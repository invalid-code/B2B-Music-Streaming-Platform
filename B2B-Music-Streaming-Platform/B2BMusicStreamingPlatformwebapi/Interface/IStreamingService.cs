namespace API.Interface
{
    public interface IStreamingService
    {
        Task<string> RequestPlaybackAsync(string venueId, string trackId, int totalPlaytimeInSeconds);
    }
}
