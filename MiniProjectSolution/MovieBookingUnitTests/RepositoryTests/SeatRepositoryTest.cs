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
    public class SeatRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private SeatRepository _seatRepository;
        private ScreenRepository _screenRepository;
        private TheaterRepository _theaterRepository;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _seatRepository = new SeatRepository(_context);
            _screenRepository = new ScreenRepository(_context);
            _theaterRepository = new TheaterRepository(_context);

            var theater = new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Vadapalani"
            };
            await _theaterRepository.Add(theater);

            var screen = new Screen
            {
                Name = "PXL",
                SeatingCapacity = 150,
                TheaterId = theater.TheaterId
            };
            await _screenRepository.Add(screen);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddSeatSuccess()
        {
            var screen = await _context.Screens.FirstAsync();
            var seat = new Seat
            {
                ScreenId = screen.ScreenId,
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                Price = 10.00m
            };

            var result = await _seatRepository.Add(seat);

            Assert.IsNotNull(result);
            Assert.AreEqual("A1", result.SeatNumber);
        }

        [Test]
        public void AddSeat_Fail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _seatRepository.Add(null));
        }

        [Test]
        public async Task GetSeatByIdSuccess()
        {
            var screen = await _context.Screens.FirstAsync();
            var seat = new Seat
            {
                ScreenId = screen.ScreenId,
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                Price = 10.00m
            };
            seat = await _seatRepository.Add(seat);

            var result = await _seatRepository.Get(seat.SeatId);

            Assert.IsNotNull(result);
            Assert.AreEqual(seat.SeatId, result.SeatId);
        }

        [Test]
        public async Task GetSeatByIdFail()
        {
            var result = await _seatRepository.Get(999);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllSeatsSuccess()
        {
            var screen = await _context.Screens.FirstAsync();
            var seat1 = new Seat
            {
                ScreenId = screen.ScreenId,
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                Price = 10.00m
            };
            var seat2 = new Seat
            {
                ScreenId = screen.ScreenId,
                SeatNumber = "A2",
                Row = "A",
                Column = 2,
                Price = 10.00m
            };
            await _seatRepository.Add(seat1);
            await _seatRepository.Add(seat2);

            var result = await _seatRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllSeatsFail()
        {
            Assert.IsNull(await _seatRepository.Get());
        }

        [Test]
        public async Task UpdateSeatSuccess()
        {
            var screen = await _context.Screens.FirstAsync();
            var seat = new Seat
            {
                ScreenId = screen.ScreenId,
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                Price = 10.00m
            };
            seat = await _seatRepository.Add(seat);
            seat.Price = 12.00m;

            var result = await _seatRepository.Update(seat);

            Assert.IsNotNull(result);
            Assert.AreEqual(seat.Price, result.Price);
        }

        [Test]
        public void UpdateSeatFail()
        {
            var seat = new Seat
            {
                SeatId = 999,
                ScreenId = 1,
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                Price = 10.00m
            };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _seatRepository.Update(seat));
        }

        [Test]
        public async Task DeleteSeatSuccess()
        {
            var screen = await _context.Screens.FirstAsync();
            var seat = new Seat
            {
                ScreenId = screen.ScreenId,
                SeatNumber = "A1",
                Row = "A",
                Column = 1,
                Price = 10.00m
            };
            seat = await _seatRepository.Add(seat);

            var result = await _seatRepository.Delete(seat.SeatId);

            Assert.IsNotNull(result);
            Assert.AreEqual(seat.SeatId, result.SeatId);
        }

        [Test]
        public void DeleteSeatFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _seatRepository.Delete(999));
        }
    }
}
