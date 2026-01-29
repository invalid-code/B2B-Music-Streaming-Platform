using API.Interface;
using API.Models.DTOs.Requests.Venue;
using API.Models.DTOs.Response.venue;
using Microsoft.AspNetCore.Mvc;


namespace MusicStreamingPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        // GET: api/venues
        [HttpGet]
        public async Task<IActionResult> GetAllVenues()
        {
            var venues = await _venueService.GetAllVenuesAsync();
            return Ok(new VenueListResponse { Venues = venues });
        }

        // GET: api/venues/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenueById(string id)
        {
            var venue = await _venueService.GetVenueByIdAsync(id);
            if (venue == null)
                return NotFound();
            return Ok(venue);
        }

        // POST: api/venues
        [HttpPost]
        public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var venueResponse = await _venueService.CreateVenueAsync(request);
            return CreatedAtAction(nameof(GetVenueById), new { id = venueResponse.VenueID }, venueResponse);
        }

        // PUT: api/venues/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenue(string id, [FromBody] UpdateVenueRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _venueService.UpdateVenueAsync(id, request);
            if (!result)
                return NotFound();
            return NoContent();
        }

        // DELETE: api/venues/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(string id)
        {
            var result = await _venueService.DeleteVenueAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
