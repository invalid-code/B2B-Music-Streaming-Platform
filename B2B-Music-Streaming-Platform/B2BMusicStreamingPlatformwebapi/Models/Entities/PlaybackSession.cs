namespace API.Models.Core_Models
{
    public class PlaybackSession
    {
        public string SessionID { get; set; }
        public string VenueID { get; set; }
        public string TrackID { get; set; }
        public DateTime Timestamp { get; set; }
        public int DurationPlayed { get; set; } // In seconds
    }

}
