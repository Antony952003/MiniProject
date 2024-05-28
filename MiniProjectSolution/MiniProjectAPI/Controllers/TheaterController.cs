using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Theater;
using System.Security.Claims;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService) {
            _theaterService = theaterService;
        }
        [HttpPost]
        [Route("AddTheater")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TheaterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<TheaterDTO>> AddTheater(TheaterDTO theaterDTO)
        {
            try
            {
                if(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value == "User")
                {
                    throw new AdminOnlyOperationException();
                }
                var result = await _theaterService.AddTheater(theaterDTO);
                return Ok(result);
            }
            catch (AdminOnlyOperationException ex)
            {
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(409, ex.Message));
            }
        }
        [HttpGet]
        [Route("GetTheaterById")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TheaterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<TheaterDTO>> GetTheaterById(int theaterid)
        {
            try
            {
                var result = await _theaterService.GetTheaterById(theaterid);
                return Ok(result);
            }
            catch(EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }

        }
        [HttpGet]
        [Route("GetTheaterByName")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(TheaterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<TheaterDTO>> GetTheaterByName(string theaterName)
        {
            try
            {
                var result = await _theaterService.GetTheaterByName(theaterName);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }

        }
        [HttpPut]
        [Route("UpdateTheaterLocation")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TheaterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesErrorResponseType(typeof(ErrorModel))]
        public async Task<ActionResult<TheaterDTO>> UpdateTheaterLocation(int theaterid, string location)
        {
            try
            {
                var result = await _theaterService.UpdateTheaterLocation(theaterid, location);
                return Ok(result);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }

        }
    }
}
