using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class TheaterRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
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

            _theaterRepository = new TheaterRepository(_context);

        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddTheaterSuccess()
        {
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };

            var result = await _theaterRepository.Add(theater);

            Assert.IsNotNull(result);
            Assert.AreEqual(theater.Name, result.Name);
        }

        [Test]
        public void AddTheaterFail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _theaterRepository.Add(null));
        }

        [Test]
        public async Task GetTheaterByIdSuccess()
        {
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            theater = await _theaterRepository.Add(theater);

            var result = await _theaterRepository.Get(theater.TheaterId);

            Assert.IsNotNull(result);
            Assert.AreEqual(theater.Name, result.Name);
        }

        [Test]
        public async Task GetTheaterByIdFail()
        {
            var result = await _theaterRepository.Get(999);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllTheatersSuccess()
        {
            var theater1 = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            var theater2 = new Theater { Name = "PVR Skyone", Location = "Aminjikarai" };
            await _theaterRepository.Add(theater1);
            await _theaterRepository.Add(theater2);

            var result = await _theaterRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllTheatersFail()
        {
            foreach (var theater in _context.Theaters)
            {
                _context.Theaters.Remove(theater);
            }
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<NoEntitiesFoundException>(async () => await _theaterRepository.Get());
        }

        [Test]
        public async Task UpdateTheaterSuccess()
        {
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            theater = await _theaterRepository.Add(theater);
            theater.Name = "Updated Cinema One";

            var result = await _theaterRepository.Update(theater);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Cinema One", result.Name);
        }

        [Test]
        public void UpdateTheaterFail()
        {
            var theater = new Theater { TheaterId = 999 ,Name = "Rohini Cinemas", Location = "Vadapalani" };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _theaterRepository.Update(theater));
        }

        [Test]
        public async Task DeleteTheaterSuccess()
        {
            var theater = new Theater { Name = "Rohini Cinemas", Location = "Vadapalani" };
            theater = await _theaterRepository.Add(theater);

            var result = await _theaterRepository.Delete(theater.TheaterId);

            Assert.IsNotNull(result);
            Assert.AreEqual(theater.Name, result.Name);
        }

        [Test]
        public void DeleteTheaterFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _theaterRepository.Delete(999));
        }
    }
}
