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
    public class ScreenRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
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

            _screenRepository = new ScreenRepository(_context);
            _theaterRepository = new TheaterRepository(_context);

            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            await _theaterRepository.Add(theater);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddScreenSuccess()
        {
            var theater = await _context.Theaters.FirstAsync();
            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 150, TheaterId = theater.TheaterId };

            var result = await _screenRepository.Add(screen);

            Assert.IsNotNull(result);
            Assert.AreEqual("Screen 1", result.Name);
        }

        [Test]
        public void AddScreenFail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _screenRepository.Add(null));
        }

        [Test]
        public async Task GetScreenByIdSuccess()
        {
            var theater = await _context.Theaters.FirstAsync();
            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 150, TheaterId = theater.TheaterId };
            screen = await _screenRepository.Add(screen);

            var result = await _screenRepository.Get(screen.ScreenId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Screen 1", result.Name);
        }

        [Test]
        public async Task GetScreenByIdFail()
        {
            Assert.IsNull(await _screenRepository.Get(999));

        }

        [Test]
        public async Task GetAllScreensSuccess()
        {
            var theater = await _context.Theaters.FirstAsync();
            var screen1 = new Screen { Name = "Screen 1", SeatingCapacity = 150, TheaterId = theater.TheaterId };
            var screen2 = new Screen { Name = "Screen 2", SeatingCapacity = 200, TheaterId = theater.TheaterId };
            await _screenRepository.Add(screen1);
            await _screenRepository.Add(screen2);

            var result = await _screenRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllScreensFail()
        {
            foreach (var screen in _context.Screens)
            {
                _context.Screens.Remove(screen);
            }
            await _context.SaveChangesAsync();
            var screens = await _screenRepository.Get();

            Assert.IsNull(screens);
        }

        [Test]
        public async Task UpdateScreenSuccess()
        {
            var theater = await _context.Theaters.FirstAsync();
            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 150, TheaterId = theater.TheaterId };
            screen = await _screenRepository.Add(screen);
            screen.Name = "Updated Screen 1";

            var result = await _screenRepository.Update(screen);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Screen 1", result.Name);
        }

        [Test]
        public void UpdateScreenFail()
        {
            var screen = new Screen { ScreenId = 999, Name = "Non-existent Screen", SeatingCapacity = 100, TheaterId = 1 };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _screenRepository.Update(screen));
        }

        [Test]
        public async Task DeleteScreenSuccess()
        {
            var theater = await _context.Theaters.FirstAsync();
            var screen = new Screen { Name = "Screen 1", SeatingCapacity = 150, TheaterId = theater.TheaterId };
            screen = await _screenRepository.Add(screen);

            var result = await _screenRepository.Delete(screen.ScreenId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Screen 1", result.Name);
        }

        [Test]
        public void DeleteScreenFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _screenRepository.Delete(999));
        }
    }
}
