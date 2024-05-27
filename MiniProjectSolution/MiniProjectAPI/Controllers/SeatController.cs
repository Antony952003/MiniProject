using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Seat;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService) {
            _seatService = seatService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<SeatOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<SeatOutputDTO>>> SeatGeneration(SeatGenerationInputDTO seatGeneration)
        {
            try
            {
                var result = await _seatService.GenerateSeatsForScreen(seatGeneration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(404, ex.Message));
            }
        }
    }
}
