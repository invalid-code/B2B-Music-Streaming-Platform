using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs.Requests.Track
{
    /// <summary>
    /// Request model for obtaining a signed URL for audio streaming.
    /// </summary>
    public class StreamAuthorizationRequest
    {
        /// <summary>
        /// The track ID to stream.
        /// </summary>
        [Required(ErrorMessage = "Track ID is required")]
        public string TrackId { get; set; }

        /// <summary>
        /// The venue ID requesting the stream.
        /// </summary>
        [Required(ErrorMessage = "Venue ID is required")]
        public string VenueId { get; set; }

        /// <summary>
        /// Optional: The expected duration of playback in seconds.
        /// </summary>
        [Range(1, 86400, ErrorMessage = "Playback duration must be between 1 second and 24 hours")]
        public int? PlaybackDuration { get; set; }
    }
}