using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;
using MovieBookingAPI.Models.DTOs.Payment;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class PaymentServiceTest : BasicSetup
    {

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            
        }

        [Test]
        public async Task MakePayment_Success()
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

            var seat = new Seat { SeatNumber = "A1", Row = "A", Column = 1, ScreenId = screen.ScreenId, Price = 100m };
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


            var paymentInputDTO = new PaymentInputDTO
            {
                BookingId = result.BookingId,
                PaymentMethod = "CreditCard",
                
            };

            // Act
            var fresult = await _paymentService.MakePayment(paymentInputDTO);

            // Assert
            Assert.IsNotNull(fresult);
            Assert.AreEqual(result.BookingId, fresult.BookingId);
            Assert.AreEqual("Success", fresult.PaymentStatus);
            Assert.AreEqual(100m + 80m + 14.40m, fresult.OrderTotal); // TicketPrice + PlatformFee + IGST
        }

        [Test]
        public void MakePayment_BookingNotFound()
        {
            // Arrange
            var paymentInputDTO = new PaymentInputDTO
            {
                BookingId = 999, // non-existent booking ID
                PaymentMethod = "CreditCard"
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _paymentService.MakePayment(paymentInputDTO));
        }

        [Test]
        public async Task MakePayment_PaymentAlreadyCompleted()
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

            var seat = new Seat { SeatNumber = "A1", Row = "A", Column = 1, ScreenId = screen.ScreenId, Price = 100m };
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


            var paymentInputDTO = new PaymentInputDTO
            {
                BookingId = result.BookingId,
                PaymentMethod = "CreditCard",

            };

            // Act
            var fresult = await _paymentService.MakePayment(paymentInputDTO);

            paymentInputDTO = new PaymentInputDTO
            {
                BookingId = result.BookingId,
                PaymentMethod = "CreditCard"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _paymentService.MakePayment(paymentInputDTO));
            Assert.AreEqual("Payment already completed for this booking.", ex.Message);
        }
    }
}
