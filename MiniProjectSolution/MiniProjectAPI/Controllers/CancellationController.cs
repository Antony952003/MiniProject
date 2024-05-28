using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Cancellation;
using System.ComponentModel;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _cancellationService;

        public CancellationController(ICancellationService cancellationService) {
            _cancellationService = cancellationService;
        }

        [HttpPost("ProcessCancellation")]
        [ProducesResponseType(typeof(CancellationReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CancellationReturnDTO>> ProcessCancellation(CancellationInputDTO cancellationInputDTO)
        {
            try
            {
                var result = await _cancellationService.ProcessCancellation(cancellationInputDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new ErrorModel(404, ex.Message));
            }
        }
    }
}
