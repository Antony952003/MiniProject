using Microsoft.AspNetCore.Authorization.Infrastructure;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Review;

namespace MovieBookingAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<int, Review> _reviewRepo;
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, Movie> _movieRepo;

        public ReviewService(IRepository<int, Review> reviewRepo,
            IRepository<int, User> userRepo,
            IRepository<int, Movie> movieRepo) { 
            _reviewRepo = reviewRepo;
            _userRepo = userRepo;
            _movieRepo = movieRepo;
        }

        public async Task<ReviewReturnDTO> GiveReviews(int userId, ReviewInputDTO reviewInputDTO)
        {
            try
            {
                var user = await _userRepo.Get(userId);
                if (user == null)
                {
                    throw new EntityNotFoundException("User");
                }
                var movie = await _movieRepo.Get(reviewInputDTO.MovieId);
                if (movie == null)
                {
                    throw new EntityNotFoundException("Movie");
                }
                Review review = new Review()
                {
                    UserId = userId,
                    MovieId = movie.MovieId,
                    Comment = reviewInputDTO.Comment,
                    Rating = reviewInputDTO.Rating,
                    CreatedAt = DateTime.UtcNow,
                };
                await _reviewRepo.Add(review);
                return new ReviewReturnDTO()
                {
                    UserId = user.Id,
                    MovieName = movie.Title,
                    Comment = reviewInputDTO.Comment,
                    Rating = reviewInputDTO.Rating,
                    CreatedAt = DateTime.UtcNow,
                    UserName = user.Name,
                };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
