namespace API.Models.Core_Models
{
    public class Track
    {
        public string TrackID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Mood { get; set; }
        public string CloudflareStorageKey { get; set; } // Encrypted key for Cloudflare R2
        public DateTime UploadedAt { get; set; }
    }

}
