namespace API.Models.DTOs.Requests.Playlist
{
    public class PlaylistResponse
    {
        public string PlaylistID { get; set; }
        public string Name { get; set; }
        public string VibeOrGenre { get; set; }
        public List<string> TrackIDs { get; set; }
    }
}
