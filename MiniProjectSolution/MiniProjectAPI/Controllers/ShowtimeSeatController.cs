using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Showtime;
using MovieBookingAPI.Models.DTOs.ShowtimeSeat;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeSeatController : ControllerBase
    {
        private readonly IShowtimeSeatService _showtimeSeatService;
        public ShowtimeSeatController(IShowtimeSeatService showtimeSeatService) { 
            _showtimeSeatService = showtimeSeatService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddShowtimeSeat")]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeSeatReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ShowtimeSeatReturnDTO>>> AddShowtimeSeats(ShowtimeSeatGenerateDTO showtimeSeatGenerateDTO)
        {
            try
            {
                var result = await _showtimeSeatService.GenerateShowtimeSeats(showtimeSeatGenerateDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}
