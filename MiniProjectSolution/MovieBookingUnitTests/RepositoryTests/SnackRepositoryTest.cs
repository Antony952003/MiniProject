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
    public class SnackRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private SnackRepository _snackRepository;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _snackRepository = new SnackRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddSnack_Success()
        {
            var snack = new Snack { Name = "Popcorn",  Price = 5.50M };
            var result = await _snackRepository.Add(snack);

            Assert.IsNotNull(result);
            Assert.AreEqual("Popcorn", result.Name);
            Assert.AreEqual(5.50M, result.Price);
        }

        [Test]
        public void AddSnack_NullItem_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _snackRepository.Add(null));
        }

        [Test]
        public async Task GetSnackById_Success()
        {
            var snack = new Snack { Name = "Popcorn", Price = 5.50M };
            snack = await _snackRepository.Add(snack);

            var result = await _snackRepository.Get(snack.SnackId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Popcorn", result.Name);
        }

        [Test]
        public void GetSnackById_NotFound_ThrowsException()
        {
            Assert.ThrowsAsync<NoEntitiesFoundException>(async () => await _snackRepository.Get(999));
        }

        [Test]
        public async Task GetAllSnacks_Success()
        {
            var snack1 = new Snack { Name = "Popcorn", Price = 5.50M };
            var snack2 = new Snack { Name = "Soda", Price = 3.00M };
            await _snackRepository.Add(snack1);
            await _snackRepository.Add(snack2);

            var result = await _snackRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllSnacks_Empty_ThrowsException()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _snackRepository.Get());
        }

        [Test]
        public async Task UpdateSnack_Success()
        {
            var snack = new Snack { Name = "Popcorn", Price = 5.50M };
            snack = await _snackRepository.Add(snack);
            snack.Price = 6.00M;

            var result = await _snackRepository.Update(snack);

            Assert.IsNotNull(result);
            Assert.AreEqual(6.00M, result.Price);
        }

        [Test]
        public void UpdateSnack_Fail()
        {
            var snack = new Snack { SnackId = 999, Name = "Popcorn", Price = 5.50M };
            Assert.ThrowsAsync<NoEntitiesFoundException>(async () => await _snackRepository.Update(snack));
        }

        [Test]
        public async Task DeleteSnack_Success()
        {
            var snack = new Snack { Name = "Popcorn", Price = 5.50M };
            snack = await _snackRepository.Add(snack);

            var result = await _snackRepository.Delete(snack.SnackId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Popcorn", result.Name);
        }

        [Test]
        public void DeleteSnack_NotFound_ThrowsException()
        {
            Assert.ThrowsAsync<NoEntitiesFoundException>(async () => await _snackRepository.Delete(999));
        }
    }
}
