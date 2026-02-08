using API.Interface;

namespace API.Models.Entities
{
    public class PlaybackLog : ITenantEntity
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string TrackId { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}
