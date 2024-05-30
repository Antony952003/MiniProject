using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models.DTOs.Movie;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MovieBookingUnitTests.ServiceTests
{
    public class MovieServiceTest : BasicSetup
    {
        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task AddMovie_Success()
        {
            // Arrange
            var movieInputDTO = new MovieInputDTO
            {
                Title = "Inception",
                Genre = "Sci-Fi",
                Description = "A mind-bending thriller",
                DurationInHours = 2.5,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt"
            };

            // Act
            var result = await _movieService.AddMovie(movieInputDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(movieInputDTO.Title, result.Title);
            Assert.AreEqual(movieInputDTO.Genre, result.Genre);
            Assert.AreEqual(movieInputDTO.Description, result.Description);
            Assert.AreEqual(movieInputDTO.DurationInHours, result.DurationInHours);
            Assert.AreEqual(movieInputDTO.Director, result.Director);
            Assert.AreEqual(movieInputDTO.Cast, result.Cast);
        }

        [Test]
        public void AddMovie_Failure_MovieAlreadyExists()
        {
            // Arrange
            var movie = new Movie
            {
                Title = "Inception",
                Genre = "Sci-Fi",
                Description = "A mind-bending thriller",
                DurationInHours = 2.5,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt"
            };
            context.Movies.Add(movie);
            context.SaveChanges();

            var movieInputDTO = new MovieInputDTO
            {
                Title = "Inception",
                Genre = "Sci-Fi",
                Description = "A mind-bending thriller",
                DurationInHours = 2.5,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt"
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExists>(() => _movieService.AddMovie(movieInputDTO));
        }

        [Test]
        public async Task SortMoviesByReviews_Success()
        {
            // Arrange
            var movie1 = new Movie
            {
                Title = "Movie 1",
                Genre = "Genre 1",
                Description = "Description 1",
                DurationInHours = 2,
                Director = "Director 1",
                Cast = "Cast 1"
            };
            var movie2 = new Movie
            {
                Title = "Movie 2",
                Genre = "Genre 2",
                Description = "Description 2",
                DurationInHours = 1.5,
                Director = "Director 2",
                Cast = "Cast 2"
            };

            var user = new User()
            {
                Email = "elonmusk@gmail.com",
                Name = "Elon Musk",
                DateOfBirth = new DateTime(2003, 05, 09),
                Image = "elon.png",
                Phone = "9080950106",
                Role = "Admin",
            };

            context.Users.Add(user);
            context.SaveChanges();
            user = await context.Users.FirstAsync();

            context.Movies.AddRange(movie1, movie2);
            context.SaveChanges();


            var review1 = new Review { UserId = user.Id,MovieId = movie1.MovieId,Comment = "Very Nice", Rating = 4 };
            var review2 = new Review { UserId = user.Id,MovieId = movie1.MovieId, Comment = "Fabulous", Rating = 5 };
            var review3 = new Review { UserId = user.Id,MovieId = movie2.MovieId, Comment = "Very good", Rating = 3 };

            context.Reviews.AddRange(review1, review2, review3);
            context.SaveChanges();

            // Act
            var result = await _movieService.SortMoviesByReviews();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Movie 1", result[0].Title);
            Assert.AreEqual("Movie 2", result[1].Title);
            Assert.AreEqual(4.5, result[0].AverageRating);
            Assert.AreEqual(3, result[1].AverageRating);
        }
    }
}
