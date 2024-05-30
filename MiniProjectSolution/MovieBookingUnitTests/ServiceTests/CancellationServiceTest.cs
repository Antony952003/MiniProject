using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;
using MovieBookingAPI.Models.DTOs.Cancellation;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class CancellationServiceTest : BasicSetup
    {

        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task ProcessCancellation_Success()
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

            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = new DateTime(2024, 05, 31, 01, 00, 00) // Setting the start time to 3 hours from now
            };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = await context.Showtimes.FirstAsync();

            var seat = new Seat
            {
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                ScreenId = screen.ScreenId,
                Price = 100m
            };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = await context.Seats.FirstAsync();

            var showtimeSeat = new ShowtimeSeat
            {
                ShowtimeId = showtime.ShowtimeId,
                SeatId = seat.SeatId,
                Status = "Available"
            };
            context.ShowtimeSeats.Add(showtimeSeat);
            await context.SaveChangesAsync();
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

            var cancellationInputDTO = new CancellationInputDTO
            {
                BookingId = result.BookingId,
                SeatNumbers = new List<string> { "A1" },
                Reason = "No reason",
                
            };

            // Act
            var fresult = await _cancellationService.ProcessCancellation(cancellationInputDTO);

            // Assert
            Assert.IsNotNull(fresult);
            Assert.AreEqual(result.BookingId, fresult.BookingId);
            Assert.AreEqual(25m, fresult.RefundAmount); // 75% refund since cancellation is more than 2 hours before showtime
            Assert.AreEqual(1, fresult.CancelledSeatNumbers.Count);
            Assert.AreEqual("A1", fresult.CancelledSeatNumbers.First());
        }

        [Test]
        public void ProcessCancellation_BookingNotFound()
        {
            // Arrange
            var cancellationInputDTO = new CancellationInputDTO
            {
                BookingId = 999, // non-existent booking ID
                SeatNumbers = new List<string> { "A1" }
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _cancellationService.ProcessCancellation(cancellationInputDTO));
        }

        [Test]
        public async Task ProcessCancellation_CancellationNotAllowed()
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

            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = new DateTime(2024, 05, 31, 00, 50, 00) // Setting the start time to 10 minutes from now
            };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = await context.Showtimes.FirstAsync();

            var seat = new Seat
            {
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                ScreenId = screen.ScreenId,
                Price = 100m
            };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = await context.Seats.FirstAsync();

            var showtimeSeat = new ShowtimeSeat
            {
                ShowtimeId = showtime.ShowtimeId,
                SeatId = seat.SeatId,
                Status = "Available"
            };
            context.ShowtimeSeats.Add(showtimeSeat);
            await context.SaveChangesAsync();
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

            var cancellationInputDTO = new CancellationInputDTO
            {
                BookingId = result.BookingId,
                SeatNumbers = new List<string> { "A1" },
                Reason = "No reason",

            };

            // Act
            // Act & Assert
            Assert.ThrowsAsync<CancellationNotAllowedException>(async () => await _cancellationService.ProcessCancellation(cancellationInputDTO));
        }

        [Test]
        public async Task ProcessCancellation_SeatNotInBooking()
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

            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = new DateTime(2024,05,31,02,00,00) // Setting the start time to 3 hours from now
            };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = await context.Showtimes.FirstAsync();

            var seat = new Seat
            {
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                ScreenId = screen.ScreenId,
                Price = 100m
            };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = await context.Seats.FirstAsync();

            var showtimeSeat = new ShowtimeSeat
            {
                ShowtimeId = showtime.ShowtimeId,
                SeatId = seat.SeatId,
                Status = "Available"
            };
            context.ShowtimeSeats.Add(showtimeSeat);
            await context.SaveChangesAsync();
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

            var cancellationInputDTO = new CancellationInputDTO
            {
                BookingId = result.BookingId,
                SeatNumbers = new List<string> { "B1" } // Seat not in the booking
            };

            // Act & Assert
            Assert.ThrowsAsync<BookingDoesNotHaveSeatException>(async () => await _cancellationService.ProcessCancellation(cancellationInputDTO));
        }
    }
}
