namespace API.Models.DTOs.Requests.Playlist
{
    public class CreatePlaylistRequest
    {
        public string Name { get; set; }
        public string VibeOrGenre { get; set; }
        public List<string> TrackIDs { get; set; }
    }
}
