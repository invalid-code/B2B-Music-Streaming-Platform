using API.Interface;
using API.Models.DTOs.Requests.Track;
using API.Models.DTOs.Response.track;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        // GET: api/tracks
        [HttpGet]
        public async Task<IActionResult> GetAllTracks()
        {
            var tracks = await _trackService.GetAllTracksAsync();
            return Ok(tracks);
        }

        // GET: api/tracks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrackById(string id)
        {
            var track = await _trackService.GetTrackByIdAsync(id);
            if (track == null)
                return NotFound(new { message = "Track not found" });
            return Ok(track);
        }

        // POST: api/tracks
        [HttpPost]
        public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trackResponse = await _trackService.CreateTrackAsync(request);
            return CreatedAtAction(nameof(GetTrackById), new { id = trackResponse.TrackID }, trackResponse);
        }

        // PUT: api/tracks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrack(string id, [FromBody] UpdateTrackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _trackService.UpdateTrackAsync(id, request);
            if (!result)
                return NotFound(new { message = "Track not found" });

            return NoContent();
        }

        // DELETE: api/tracks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(string id)
        {
            var result = await _trackService.DeleteTrackAsync(id);
            if (!result)
                return NotFound(new { message = "Track not found" });

            return NoContent();
        }

        /// <summary>
        /// Uploads a new track with audio file to Cloudflare R2 storage.
        /// </summary>
        /// <param name="request">The upload request containing track metadata and audio file</param>
        /// <param name="r2Service">The Cloudflare R2 service for file upload</param>
        /// <returns>The upload response with track details</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadTrack(
            [FromForm] UploadTrackRequest request,
            [FromServices] ICloudflareR2Service r2Service)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var logger = HttpContext.RequestServices.GetService<ILogger<TrackService>>();
            var response = await _trackService.UploadTrackAsync(request, r2Service, logger);

            if (response.Success)
                return CreatedAtAction(nameof(GetTrackById), new { id = response.TrackID }, response);

            return BadRequest(new { message = response.ErrorMessage });
        }

        /// <summary>
        /// Generates a signed URL for secure audio streaming.
        /// </summary>
        /// <param name="request">The stream authorization request</param>
        /// <param name="r2Service">The Cloudflare R2 service for signed URL generation</param>
        /// <returns>The stream authorization response with signed URL</returns>
        [HttpPost("authorize-stream")]
        public async Task<IActionResult> AuthorizeStream(
            [FromBody] StreamAuthorizationRequest request,
            [FromServices] ICloudflareR2Service r2Service)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var logger = HttpContext.RequestServices.GetService<ILogger<TrackService>>();
            var response = await _trackService.AuthorizeStreamAsync(request, r2Service, logger);

            if (response.Success)
                return Ok(response);

            return BadRequest(new { message = response.ErrorMessage });
        }
    }
}
