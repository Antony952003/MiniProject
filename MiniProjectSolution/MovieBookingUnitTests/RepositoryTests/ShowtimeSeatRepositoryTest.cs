using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class ShowtimeSeatRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private ShowtimeSeatRepository _showtimeSeatRepository;
        private TheaterRepository _theaterRepository;
        private ScreenRepository _screenRepository;
        private SeatRepository _seatRepository;
        private ShowtimeRepository _showtimeRepository;
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
            _theaterRepository = new TheaterRepository(_context);
            _screenRepository = new ScreenRepository(_context);
            _seatRepository = new SeatRepository(_context);
            _showtimeRepository = new ShowtimeRepository(_context);
            _movieRepository = new MovieRepository(_context);

            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            theater = await _theaterRepository.Add(theater);
            // Add some seats
            var screen = new Screen {  Name ="PXL", SeatingCapacity=230, TheaterId = theater.TheaterId };
            screen = await _screenRepository.Add(screen);
            var seat1 = new Seat {  ScreenId = screen.ScreenId, SeatNumber = "A1", Row = "A", Column = 1, Price = 170 };
            var seat2 = new Seat {  ScreenId = screen.ScreenId, SeatNumber = "A2", Row = "A", Column = 2, Price = 170 };
            seat1 = await _seatRepository.Add(seat1);
            seat2 = await _seatRepository.Add(seat2);
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

            movie = await _movieRepository.Add(movie);
            var showtime = new Showtime() { ScreenId = screen.ScreenId, MovieId = movie.MovieId,StartTime = Convert.ToDateTime("2024-05-16T05:50:06") };
            showtime = await _showtimeRepository.Add(showtime);
            await _context.SaveChangesAsync();

            _showtimeSeatRepository = new ShowtimeSeatRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddShowtimeSeatSuccess()
        {
            // Assuming a showtime with ID 1 exists
            var showtimeSeat = new ShowtimeSeat
            {
                Status = "Available",
                SeatId = 1,
                ShowtimeId = 1,
                
            };

            var result = await _showtimeSeatRepository.Add(showtimeSeat);

            Assert.IsNotNull(result);
            Assert.AreEqual("Available", result.Status);
        }

        [Test]
        public void AddShowtimeSeat_Fail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _showtimeSeatRepository.Add(null));
        }

        [Test]
        public async Task GetShowtimeSeatByIdSuccess()
        {
            // Assuming a showtime seat with ID 1 exists
            var showtimeSeat = new ShowtimeSeat
            {
                Status = "Available",
                SeatId = 1,
                ShowtimeId = 1
            };
            showtimeSeat = await _showtimeSeatRepository.Add(showtimeSeat);

            var result = await _showtimeSeatRepository.Get(showtimeSeat.ShowtimeSeatId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Available", result.Status);
        }

        [Test]
        public async Task GetShowtimeSeatByIdFail()
        {
            Assert.IsNull(await _showtimeSeatRepository.Get(999));
        }

        [Test]
        public async Task GetAllShowtimeSeatsSuccess()
        {
            var showtimeSeat1 = new ShowtimeSeat
            {
                Status = "Available",
                SeatId = 1,
                ShowtimeId = 1
            };
            await _showtimeSeatRepository.Add(showtimeSeat1);

            var result = await _showtimeSeatRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetAllShowtimeSeats_Fail()
        {
            Assert.IsNull(await _showtimeSeatRepository.Get());
        }

        [Test]
        public async Task UpdateShowtimeSeatSuccess()
        {
            // Assuming a showtime seat with ID 1 exists
            var showtimeSeat = new ShowtimeSeat
            {
                Status = "Available",
                SeatId = 1,
                ShowtimeId = 1
            };
            showtimeSeat = await _showtimeSeatRepository.Add(showtimeSeat);
            showtimeSeat.Status = "Booked";

            var result = await _showtimeSeatRepository.Update(showtimeSeat);

            Assert.IsNotNull(result);
            Assert.AreEqual("Booked", result.Status);
        }

        [Test]
        public void UpdateShowtimeSeatFail()
        {
            var showtimeSeat = new ShowtimeSeat
            {
                ShowtimeSeatId = 999,
                Status = "Available",
                SeatId = 1,
                ShowtimeId = 1
            };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _showtimeSeatRepository.Update(showtimeSeat));
        }

        [Test]
        public async Task DeleteShowtimeSeatSuccess()
        {
            // Assuming a showtime seat with ID 1 exists
            var showtimeSeat = new ShowtimeSeat
            {
                Status = "Available",
                SeatId = 1,
                ShowtimeId = 1
            };
            showtimeSeat = await _showtimeSeatRepository.Add(showtimeSeat);

            var result = await _showtimeSeatRepository.Delete(showtimeSeat.ShowtimeSeatId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Available", result.Status);
        }

        [Test]
        public void DeleteShowtimeSeatFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _showtimeSeatRepository.Delete(999));
        }
    }
}
