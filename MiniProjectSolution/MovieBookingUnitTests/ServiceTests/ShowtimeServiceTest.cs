using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models.DTOs.Showtime;
using MovieBookingAPI.Models.DTOs.Theater;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class ShowtimeServiceTest : BasicSetup
    {
        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task AddShowtime_Success()
        {
            // Arrange

            var theater = new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu",
            };
            context.Theaters.Add(theater);
            context.SaveChanges();
            var movie = new Movie
            {
                Title = "Inception",
                Genre = "Sci-Fi",
                Description = "A mind-bending thriller",
                DurationInHours = 2.5,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt"
            };
            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = 1
            };
            context.Movies.Add(movie);
            context.Screens.Add(screen);
            context.SaveChanges();

            var showtimeInputDTO = new ShowtimeInputDTO
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Act
            var result = await _showtimeService.AddShowtime(showtimeInputDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(movie.Title, result.MovieName);
            Assert.AreEqual(screen.Name, result.ScreenName);
            Assert.AreEqual(DateTime.Parse(showtimeInputDTO.StartTime), result.StartTime);
        }

        [Test]
        public void AddShowtime_Failure_InvalidMovie()
        {

            // Arrange
            var theater = new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu",
            };
            context.Theaters.Add(theater);
            context.SaveChanges();
            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = 1
            };
            context.Screens.Add(screen);
            context.SaveChanges();

            var showtimeInputDTO = new ShowtimeInputDTO
            {
                MovieId = 999, // Invalid movie ID
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _showtimeService.AddShowtime(showtimeInputDTO));
        }

        [Test]
        public void AddShowtime_Failure_InvalidScreen()
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

            var showtimeInputDTO = new ShowtimeInputDTO
            {
                MovieId = movie.MovieId,
                ScreenId = 999, // Invalid screen ID
                StartTime = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _showtimeService.AddShowtime(showtimeInputDTO));
        }
    }
}
