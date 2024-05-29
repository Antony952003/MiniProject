using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Movie;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService) {
            _movieService = movieService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddMovie")]
        [ProducesResponseType(typeof(MovieReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MovieReturnDTO>> AddMovie(MovieInputDTO movieInput)
        {
            try
            {
                var result = await _movieService.AddMovie(movieInput);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new ErrorModel(401, ex.Message));
            }

        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetMostReviewedMovies")]
        [ProducesResponseType(typeof(List<MovieReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<MovieReturnDTO>>> GetMoviesReviewed()
        {
            try
            {
                var result = await _movieService.SortMoviesByReviews();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(401, ex.Message));
            }

        }

    }
}
