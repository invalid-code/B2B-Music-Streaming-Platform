using API.Interface;
using API.Models.DTOs.Requests.Playlist;
using API.Models.DTOs.Response.playlist;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        // GET: api/playlists
        [HttpGet]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var playlists = await _playlistService.GetAllPlaylistsAsync();
            return Ok(playlists);
        }

        // GET: api/playlists/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistById(string id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null)
                return NotFound(new { message = "Playlist not found" });
            return Ok(playlist);
        }

        // POST: api/playlists
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var playlistResponse = await _playlistService.CreatePlaylistAsync(request);
            return CreatedAtAction(nameof(GetPlaylistById), new { id = playlistResponse.PlaylistID }, playlistResponse);
        }

        // PUT: api/playlists/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylist(string id, [FromBody] UpdatePlaylistRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _playlistService.UpdatePlaylistAsync(id, request);
            if (!result)
                return NotFound(new { message = "Playlist not found" });

            return NoContent();
        }

        // DELETE: api/playlists/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(string id)
        {
            var result = await _playlistService.DeletePlaylistAsync(id);
            if (!result)
                return NotFound(new { message = "Playlist not found" });

            return NoContent();
        }
    }
}
