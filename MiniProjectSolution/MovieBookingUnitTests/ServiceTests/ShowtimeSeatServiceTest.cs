using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models.DTOs.ShowtimeSeat;
using MovieBookingAPI.Models;
using MovieBookingAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class ShowtimeSeatServiceTest : BasicSetup
    {

        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task GenerateShowtimeSeats_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani"};
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = context.Theaters.FirstOrDefault();

            var screen = new Screen {  Name = "Screen 1", SeatingCapacity = 100,TheaterId = theater.TheaterId };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = context.Screens.FirstOrDefault();

            var seat = new Seat {  ScreenId = screen.ScreenId, SeatNumber = "A1", Row = "A", Column = 1, Price = 100m };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = context.Seats.FirstOrDefault();

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
            movie = context.Movies.FirstOrDefault();

            var showtime = new Showtime { ScreenId = screen.ScreenId, StartTime = DateTime.Now, MovieId = movie.MovieId};
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = context.Showtimes.FirstOrDefault();

            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = screen.ScreenId, ShowtimeId = showtime.ShowtimeId, };

            // Act
            var result = await _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            var firstSeat = result.First();
            Assert.AreEqual("A1", firstSeat.SeatNumber);
            Assert.AreEqual("Available", firstSeat.Status);
        }

        [Test]
        public void GenerateShowtimeSeats_ScreenNotFound()
        {
            // Arrange
            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = 999, ShowtimeId = 1};

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO));
        }

        [Test]
        public async Task GetAvailableShowtimeSeats_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = context.Theaters.FirstOrDefault();

            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 100, TheaterId = theater.TheaterId };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = context.Screens.FirstOrDefault();

            var seat = new Seat { ScreenId = screen.ScreenId, SeatNumber = "A1", Row = "A", Column = 1, Price = 100m };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = context.Seats.FirstOrDefault();

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
            movie = context.Movies.FirstOrDefault();

            var showtime = new Showtime { ScreenId = screen.ScreenId, StartTime = DateTime.Now, MovieId = movie.MovieId };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = context.Showtimes.FirstOrDefault();

            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = screen.ScreenId, ShowtimeId = showtime.ShowtimeId, };

            var showtimeseats = _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO);
            // Act
            var result = await _showtimeSeatService.GetAvailableShowtimeSeats(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("A1", result.First().SeatNumber);
        }

        [Test]
        public void GetAvailableShowtimeSeats_NoSeatsAvailable()
        {
            // Arrange & Act
            var result = _showtimeSeatService.GetAvailableShowtimeSeats(999);

            // Assert
            Assert.ThrowsAsync<NoEntitiesFoundException>(() => result);
        }

        [Test]
        public async Task GetConsecutiveSeatsInRange_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = context.Theaters.FirstOrDefault();

            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 100, TheaterId = theater.TheaterId };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = context.Screens.FirstOrDefault();

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
            movie = context.Movies.FirstOrDefault();

            var showtime = new Showtime { ScreenId = screen.ScreenId, StartTime = DateTime.Now, MovieId = movie.MovieId };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = context.Showtimes.FirstOrDefault();


            for (int i = 1; i <= 5; i++)
            {
                context.Seats.Add(new Seat {  ScreenId = screen.ScreenId, SeatNumber = $"A{i}", Row = "A", Column = i, Price = 100m });
                await context.SaveChangesAsync();
            }

            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = screen.ScreenId, ShowtimeId = showtime.ShowtimeId, };

            var chut = await _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO);

            var showtimeSeatConsecutiveRangeDTO = new ShowtimeSeatConsecutiveRangeDTO { ShowtimeId = 1, StartRow = "A", EndRow = "A", NumberOfSeats = 3 };

            // Act
            var result = await _showtimeSeatService.GetConsecutiveSeatsInRange(showtimeSeatConsecutiveRangeDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Count);
            Assert.AreEqual(3, result.First().Count);
        }

        [Test]
        public async Task GetShowtimeIdsForSeatNumbers_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = context.Theaters.FirstOrDefault();

            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 100, TheaterId = theater.TheaterId };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = context.Screens.FirstOrDefault();

            var seat = new Seat { ScreenId = screen.ScreenId, SeatNumber = "A1", Row = "A", Column = 1, Price = 100m };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = context.Seats.FirstOrDefault();

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
            movie = context.Movies.FirstOrDefault();

            var showtime = new Showtime { ScreenId = screen.ScreenId, StartTime = DateTime.Now, MovieId = movie.MovieId };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = context.Showtimes.FirstOrDefault();

            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = screen.ScreenId, ShowtimeId = showtime.ShowtimeId, };

            var res = await _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO);
            // Act
            var result = await _showtimeSeatService.GetShowtimeIdsForSeatNumbers(new List<string> { "A1" }, 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void GetShowtimeIdsForSeatNumbers_NoSeatsFound()
        {
            // Arrange
            var showtimeId = 999;
            var seatNumbers = new List<string> { "A1" };

            // Act & Assert
            Assert.ThrowsAsync<NoEntitiesFoundException>(() => _showtimeSeatService.GetShowtimeIdsForSeatNumbers(seatNumbers, showtimeId));
        }

        [Test]
        public async Task UpdateShowtimeSeatsStatus_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = context.Theaters.FirstOrDefault();

            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 100, TheaterId = theater.TheaterId };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = context.Screens.FirstOrDefault();

            var seat = new Seat { ScreenId = screen.ScreenId, SeatNumber = "A1", Row = "A", Column = 1, Price = 100m };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = context.Seats.FirstOrDefault();

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
            movie = context.Movies.FirstOrDefault();

            var showtime = new Showtime { ScreenId = screen.ScreenId, StartTime = DateTime.Now, MovieId = movie.MovieId };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = context.Showtimes.FirstOrDefault();

            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = screen.ScreenId, ShowtimeId = showtime.ShowtimeId, };

            var res = await _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO);

            // Act
            var result = await _showtimeSeatService.UpdateShowtimeSeatsStatus(1, new List<int> { 1 }, "Booked");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Booked", result.First().Status);
        }

        [Test]
        public void UpdateShowtimeSeatsStatus_NoSeatsFound()
        {
            // Arrange
            var showtimeId = 999;
            var showtimeSeatIds = new List<int> { 1 };
            var status = "Booked";

            // Act & Assert
            Assert.ThrowsAsync<NoEntitiesFoundException>(() => _showtimeSeatService.UpdateShowtimeSeatsStatus(showtimeId, showtimeSeatIds, status));
        }

        [Test]
        public async Task UpdateShowtimeSeatsStatus_TicketAlreadyBooked()
        {
            // Arrange
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            context.Theaters.Add(theater);
            await context.SaveChangesAsync();
            theater = context.Theaters.FirstOrDefault();

            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 100, TheaterId = theater.TheaterId };
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            screen = context.Screens.FirstOrDefault();

            var seat = new Seat { ScreenId = screen.ScreenId, SeatNumber = "A1", Row = "A", Column = 1, Price = 100m };
            context.Seats.Add(seat);
            await context.SaveChangesAsync();
            seat = context.Seats.FirstOrDefault();

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
            movie = context.Movies.FirstOrDefault();

            var showtime = new Showtime { ScreenId = screen.ScreenId, StartTime = DateTime.Now, MovieId = movie.MovieId };
            context.Showtimes.Add(showtime);
            await context.SaveChangesAsync();
            showtime = context.Showtimes.FirstOrDefault();

            var showtimeseatGenerateDTO = new ShowtimeSeatGenerateDTO { ScreenId = screen.ScreenId, ShowtimeId = showtime.ShowtimeId, };
            var res = await _showtimeSeatService.GenerateShowtimeSeats(showtimeseatGenerateDTO);

            // Act & Assert
            Assert.ThrowsAsync<TicketAlreadyBookedException>(() => _showtimeSeatService.UpdateShowtimeSeatsStatus(1, new List<int> { 1 }, "Available"));
        }

    }
        
}
