using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs.Requests.Track
{
    /// <summary>
    /// Request model for uploading a new track with audio file.
    /// </summary>
    public class UploadTrackRequest
    {
        /// <summary>
        /// The title of the track.
        /// </summary>
        [Required(ErrorMessage = "Track title is required")]
        [StringLength(256, ErrorMessage = "Track title cannot exceed 256 characters")]
        public string Title { get; set; }

        /// <summary>
        /// The artist name.
        /// </summary>
        [Required(ErrorMessage = "Artist name is required")]
        [StringLength(256, ErrorMessage = "Artist name cannot exceed 256 characters")]
        public string Artist { get; set; }

        /// <summary>
        /// The mood or genre of the track.
        /// </summary>
        [StringLength(100, ErrorMessage = "Mood cannot exceed 100 characters")]
        public string Mood { get; set; }

        /// <summary>
        /// The audio file to upload.
        /// </summary>
        [Required(ErrorMessage = "Audio file is required")]
        public IFormFile AudioFile { get; set; }
    }
}