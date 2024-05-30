using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;
using MovieBookingAPI.Models.DTOs.Theater;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class BookingServiceTest : BasicSetup
    {
        private BookingService _bookingService;

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            _transactionService = new TransactionRepository(context);
            _pointsService = new PointsService(_userPointRepository);
            _showtimeSeatService = new ShowtimeSeatService(_showtimeSeatRepo,_screenRepo, _seatRepo);
            _bookingSnackService = new BookingSnackService(_bookingSnackRepo);
            _snackService = new SnackService(_snackRepo);

            _bookingService = new BookingService(
                _showtimeSeatRepo, _userRepo, _screenRepo, _showtimeRepo, _bookingRepo,
                _movieRepo, _seatRepo, _snackRepo, _theaterRepo, _userPointRepository,
                _pointsService, _showtimeSeatService, _bookingSnackService, _snackService, _transactionService);
        }

        [Test]
        public async Task MakeBooking_Success()
        {
            // Arrange
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106",
                Image = "elon.png",
                Role = "User",
                
                
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            user = await context.Users.FirstAsync();
            var theater = new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu",
            };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = await context.Theaters.FirstAsync();
            var screen = new Screen
            {
                ScreenId = 1,
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId,
                
            };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = await context.Screens.FirstAsync();
            var movie = new Movie {
                Title = "Inception",
                Genre = "Sci-Fi",
                Description = "A mind-bending thriller",
                DurationInHours = 2.5,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt",
            };
            context.Movies.Add(movie);
            await context.SaveChangesAsync();
            movie = await context.Movies.FirstAsync();
            var showtime = new Showtime { MovieId = movie.MovieId, ScreenId = screen.ScreenId, StartTime = DateTime.UtcNow };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = await context.Showtimes.FirstAsync();

            var seat = new Seat { SeatNumber = "A1", Row= "A" , Column=1, ScreenId = screen.ScreenId, Price = 100m };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = await context.Seats.FirstAsync();

            var showtimeSeat = new ShowtimeSeat { ShowtimeId = showtime.ShowtimeId, SeatId = seat.SeatId, Status = "Available" };
            context.ShowtimeSeats.Add(showtimeSeat);
            context.SaveChanges();
            showtimeSeat = await context.ShowtimeSeats.FirstAsync();

            var bookingInputDTO = new BookingInputDTO
            {
                UserId = user.Id,
                ShowtimeId = showtime.ShowtimeId,
                BookingSeats = new List<string> { "A1" },
                BookingSnacks = new Dictionary<string, int>(),
                MovieId = movie.MovieId,
            };

            // Act
            var result = await _bookingService.MakeBooking(bookingInputDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(theater.Name, result.TheaterName);
            Assert.AreEqual(screen.Name, result.ScreenName);
            Assert.AreEqual(100m, result.TotalPrice);
            Assert.AreEqual("Pending", result.PaymentStatus);
        }

        [Test]
        public void MakeBooking_UserNotFound()
        {
            // Arrange
            var bookingInputDTO = new BookingInputDTO
            {
                UserId = 999, // non-existent user ID
                ShowtimeId = 1,
                BookingSeats = new List<string> { "A1" },
                BookingSnacks = new Dictionary<string, int>(),
                PointsToRedeem = 0
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _bookingService.MakeBooking(bookingInputDTO));
        }

        [Test]
        public async Task MakeBooking_ShowtimeNotFound()
        {
            // Arrange
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106",
                Image="elon.png",
                Role = "User"

            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            user = await context.Users.FirstAsync();


            var bookingInputDTO = new BookingInputDTO
            {
                UserId = user.Id,
                ShowtimeId = 999, // non-existent showtime ID
                BookingSeats = new List<string> { "A1" },
                BookingSnacks = new Dictionary<string, int>(),
                MovieId = 1,
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _bookingService.MakeBooking(bookingInputDTO));
        }

        [Test]
        public async Task MakeBooking_SeatNotFound()
        {
            // Arrange
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106",
                Image = "elon.png",
                Role = "User",


            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            user = await context.Users.FirstAsync();
            var theater = new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu",
            };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = await context.Theaters.FirstAsync();
            var screen = new Screen
            {
                ScreenId = 1,
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId,

            };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = await context.Screens.FirstAsync();
            var movie = new Movie
            {
                Title = "Inception",
                Genre = "Sci-Fi",
                Description = "A mind-bending thriller",
                DurationInHours = 2.5,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt",
            };
            context.Movies.Add(movie);
            await context.SaveChangesAsync();
            movie = await context.Movies.FirstAsync();
            var showtime = new Showtime { MovieId = movie.MovieId, ScreenId = screen.ScreenId, StartTime = DateTime.UtcNow };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = await context.Showtimes.FirstAsync();


            var bookingInputDTO = new BookingInputDTO
            {
                UserId = user.Id,
                ShowtimeId = showtime.ShowtimeId,
                BookingSeats = new List<string> { "Z1" }, // non-existent seat number
                BookingSnacks = new Dictionary<string, int>(),
                PointsToRedeem = 0
            };

            // Act & Assert
            Assert.ThrowsAsync<NoEntitiesFoundException>(() => _bookingService.MakeBooking(bookingInputDTO));
        }
    }
}
