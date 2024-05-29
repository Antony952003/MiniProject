using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class ReviewRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private ReviewRepository _reviewRepository;
        private UserRepository _userRepository;
        private MovieRepository _movieRepository;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _reviewRepository = new ReviewRepository(_context);
            _userRepository = new UserRepository(_context);
            _movieRepository = new MovieRepository(_context);

            // Add initial data for user and movie as they are required for creating reviews
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "90809501016"
            };
            await _userRepository.Add(user);

            var movie = new Movie()
            {
                Title = "Fight Club",
                Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an " +
                 "underground fight club with Tyler, a careless soap salesman",
                Cast = "Edward Norton, Brad Pitt, Jared Leto",
                Director = "David Fincher",
                DurationInHours = 2.5,
                Genre = "Thriller",
            };
            await _movieRepository.Add(movie);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddReviewSuccess()
        {
            var user = await _context.Users.FirstAsync();
            var movie = await _context.Movies.FirstAsync();
            var review = new Review
            {
                UserId = user.Id,
                MovieId = movie.MovieId,
                Comment = "Great movie!",
                Rating = 5,
                CreatedAt = DateTime.Now
            };

            var result = await _reviewRepository.Add(review);

            Assert.IsNotNull(result);
            Assert.AreEqual("Great movie!", result.Comment);
        }

        [Test]
        public void AddReview_NullReview_ShouldThrowException_Fail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _reviewRepository.Add(null));
        }

        [Test]
        public async Task GetReviewByIdSuccess()
        {
            var user = await _context.Users.FirstAsync();
            var movie = await _context.Movies.FirstAsync();
            var review = new Review
            {
                UserId = user.Id,
                MovieId = movie.MovieId,
                Comment = "Great movie!",
                Rating = 5,
                CreatedAt = DateTime.Now
            };
            review = await _reviewRepository.Add(review);

            var result = await _reviewRepository.Get(review.ReviewId);

            Assert.IsNotNull(result);
            Assert.AreEqual(review.ReviewId, result.ReviewId);
        }

        [Test]
        public async Task GetReviewByIdFail()
        {
            Assert.IsNull(await _reviewRepository.Get(999));
        }

        [Test]
        public async Task GetAllReviewsSuccess()
        {
            var user = await _context.Users.FirstAsync();
            var movie = await _context.Movies.FirstAsync();
            var review1 = new Review
            {
                UserId = user.Id,
                MovieId = movie.MovieId,
                Comment = "Great movie!",
                Rating = 5,
                CreatedAt = DateTime.Now
            };
            var review2 = new Review
            {
                UserId = user.Id,
                MovieId = movie.MovieId,
                Comment = "Not bad",
                Rating = 3,
                CreatedAt = DateTime.Now
            };
            await _reviewRepository.Add(review1);
            await _reviewRepository.Add(review2);

            var result = await _reviewRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllReviews_NoReviews_ShouldThrowNoEntitiesFoundException_Fail()
        {
            Assert.IsNull(await _reviewRepository.Get());
        }

        [Test]
        public async Task UpdateReviewSuccess()
        {
            var user = await _context.Users.FirstAsync();
            var movie = await _context.Movies.FirstAsync();
            var review = new Review
            {
                UserId = user.Id,
                MovieId = movie.MovieId,
                Comment = "Great movie!",
                Rating = 5,
                CreatedAt = DateTime.Now
            };
            review = await _reviewRepository.Add(review);
            review.Comment = "Amazing movie!";
            review.Rating = 4;

            var result = await _reviewRepository.Update(review);

            Assert.IsNotNull(result);
            Assert.AreEqual("Amazing movie!", result.Comment);
            Assert.AreEqual(4, result.Rating);
        }

        [Test]
        public void UpdateReviewFail()
        {
            var review = new Review
            {
                ReviewId = 999,
                UserId = 1,
                MovieId = 1,
                Comment = "Great movie!",
                Rating = 5,
                CreatedAt = DateTime.Now
            };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _reviewRepository.Update(review));
        }

        [Test]
        public async Task DeleteReviewSuccess()
        {
            var user = await _context.Users.FirstAsync();
            var movie = await _context.Movies.FirstAsync();
            var review = new Review
            {
                UserId = user.Id,
                MovieId = movie.MovieId,
                Comment = "Great movie!",
                Rating = 5,
                CreatedAt = DateTime.Now
            };
            review = await _reviewRepository.Add(review);

            var result = await _reviewRepository.Delete(review.ReviewId);

            Assert.IsNotNull(result);
            Assert.AreEqual(review.ReviewId, result.ReviewId);
        }

        [Test]
        public void DeleteReviewFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _reviewRepository.Delete(999));
        }
    }
}
