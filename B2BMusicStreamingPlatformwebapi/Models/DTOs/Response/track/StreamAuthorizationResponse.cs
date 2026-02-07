using System;

namespace API.Models.DTOs.Response.track
{
    /// <summary>
    /// Response model for stream authorization requests.
    /// </summary>
    public class StreamAuthorizationResponse
    {
        /// <summary>
        /// The track ID that was authorized for streaming.
        /// </summary>
        public string TrackId { get; set; }

        /// <summary>
        /// The venue ID that requested the stream.
        /// </summary>
        public string VenueId { get; set; }

        /// <summary>
        /// The signed URL for secure audio playback.
        /// </summary>
        public string SignedUrl { get; set; }

        /// <summary>
        /// The expiration time of the signed URL.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// The duration for which the stream is authorized (in seconds).
        /// </summary>
        public int AuthorizedDuration { get; set; }

        /// <summary>
        /// Indicates whether the authorization was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Any error messages if the authorization failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}