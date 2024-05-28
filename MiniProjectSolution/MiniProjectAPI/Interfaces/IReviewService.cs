using MovieBookingAPI.Models.DTOs.Review;

namespace MovieBookingAPI.Interfaces
{
    public interface IReviewService
    {
        public Task<ReviewReturnDTO> GiveReviews(int userId, ReviewInputDTO reviewInputDTO);
    }
}
