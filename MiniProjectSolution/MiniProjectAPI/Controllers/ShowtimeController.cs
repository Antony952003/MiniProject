using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Showtime;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(IShowtimeService showtimeService) {
            _showtimeService = showtimeService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ShowtimeReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ShowtimeReturnDTO>> AddShowtime(ShowtimeInputDTO showtimeInputDTO)
        {
            try
            {
                var result = await _showtimeService.AddShowtime(showtimeInputDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

    }
}
