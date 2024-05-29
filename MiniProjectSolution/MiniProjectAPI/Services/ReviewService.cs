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
        /// <summary>
        /// Retrieves reviews for a movie based on its name.
        /// </summary>
        /// <param name="moviename">The name of the movie for which reviews are to be retrieved.</param>
        /// <returns>A list of review return DTOs containing information about reviews for the specified movie.</returns>

        public async Task<List<ReviewReturnDTO>> GetMovieReviews(string moviename)
        {
            var movies = await _movieRepo.Get();
            var movie = movies.ToList().FirstOrDefault(x => x.Title == moviename);
            var reviews = await _reviewRepo.Get();
            reviews = reviews.Where(x => x.MovieId == movie.MovieId);
            List<ReviewReturnDTO> movieReviews = new List<ReviewReturnDTO>();
            foreach (var review in reviews)
            {
                var user = await _userRepo.Get(review.UserId);
                ReviewReturnDTO result = MapWithMovieReturnDTO(review, user, movie);
                movieReviews.Add(result);
            }
            return movieReviews;
            
        }
        /// <summary>
        /// Allows a user to give a review for a movie.
        /// </summary>
        /// <param name="userId">The ID of the user giving the review.</param>
        /// <param name="reviewInputDTO">The review input DTO containing information about the review to be given.</param>
        /// <returns>A review return DTO containing information about the given review.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the specified user or movie is not found.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs while giving the review.</exception>

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
                return MapWithMovieReturnDTO(review, user, movie);
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Maps a review, user, and movie to a review return DTO.
        /// </summary>
        /// <param name="review">The review entity.</param>
        /// <param name="user">The user entity associated with the review.</param>
        /// <param name="movie">The movie entity associated with the review.</param>
        /// <returns>A review return DTO containing information mapped from the provided entities.</returns>

        public ReviewReturnDTO MapWithMovieReturnDTO(Review review, User user, Movie movie)
        {
            return new ReviewReturnDTO()
            {
                UserId = review.UserId,
                MovieName = movie.Title,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = DateTime.UtcNow,
                UserName = user.Name,
            };
        }
    }
}
