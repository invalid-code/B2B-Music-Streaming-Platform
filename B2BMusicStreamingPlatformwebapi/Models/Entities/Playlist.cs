namespace API.Models.Core_Models
{
    public class Playlist
    {
        public string PlaylistID { get; set; }
        public string Name { get; set; }
        public string VibeOrGenre { get; set; }
        public List<string> TrackIDs { get; set; } // References to Track objects
        public DateTime CreatedAt { get; set; }
    }

}
