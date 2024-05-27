using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProjectAPI.Models;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Snack;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnackController : ControllerBase
    {
        private readonly ISnackService _snackService;

        public SnackController(ISnackService snackService) { 
            _snackService = snackService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(Snack), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Snack>> AddSnacks(SnackInputDTO snackInputDTO)
        {
            try
            {
                var result = await _snackService.AddSnack(snackInputDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}
