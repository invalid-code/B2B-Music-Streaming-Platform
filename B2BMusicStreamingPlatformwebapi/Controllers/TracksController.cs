using API.Interface;
using API.Models.DTOs.Requests.Track;
using API.Models.DTOs.Response.track;
using Microsoft.AspNetCore.Mvc;

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
    }
}
