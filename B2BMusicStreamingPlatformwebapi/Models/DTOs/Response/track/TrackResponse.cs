namespace API.Models.DTOs.Response.track
{
    public class TrackResponse
    {
        public string TrackID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Mood { get; set; }
        public string CloudflareStorageKey { get; set; }
    }
}
