using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Review;
using System.Security.Claims;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService) { 
            _reviewService = reviewService;
        }

        [Authorize(Roles = "User")]
        [HttpPost("GiveReview")]
        [ProducesResponseType(typeof(ReviewReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReviewReturnDTO>> GiveReviews(ReviewInputDTO reviewInputDTO)
        {
            try
            {
                var userstring = User.Claims.FirstOrDefault(x => x.Type == "uid").Value;
                int userid = Convert.ToInt32(userstring);
                var result = await _reviewService.GiveReviews(userid, reviewInputDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}
