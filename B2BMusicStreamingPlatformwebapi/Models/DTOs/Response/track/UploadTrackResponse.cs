namespace API.Models.DTOs.Response.track
{
    /// <summary>
    /// Response model for track upload operations.
    /// </summary>
    public class UploadTrackResponse
    {
        /// <summary>
        /// The unique identifier of the uploaded track.
        /// </summary>
        public string TrackID { get; set; }

        /// <summary>
        /// The title of the uploaded track.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The artist name.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// The mood or genre of the track.
        /// </summary>
        public string Mood { get; set; }

        /// <summary>
        /// The R2 storage key where the audio file is stored.
        /// </summary>
        public string CloudflareStorageKey { get; set; }

        /// <summary>
        /// The timestamp when the track was uploaded.
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// Indicates whether the upload was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Any error messages if the upload failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}