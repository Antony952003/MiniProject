using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models;
using MovieBookingAPI.Repositories;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class UserPointRepositoryTests
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private UserPointRepository _userPointRepository;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            // Add a user for the foreign key constraint
            var user = new User
            {
                Email = "elonmusk@gmail.com",
                Name = "Elon Musk",
                DateOfBirth = new DateTime(2003,05,09),
                Image = "elon.png",
                Phone = "9080950106",
                Role = "Admin",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _userPointRepository = new UserPointRepository(_context);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Test]
        public async Task AddUserPoint_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow,
            };

            var result = await _userPointRepository.Add(userPoint);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(100, result.Points);
        }

        [Test]
        public async Task GetUserPointById_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow
            };

            await _userPointRepository.Add(userPoint);

            var result = await _userPointRepository.Get(userPoint.UserPointsId);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(100, result.Points);
        }

        [Test]
        public async Task GetUserPointById_Fail()
        {
            Assert.IsNull(await _userPointRepository.Get(999));
        }

        [Test]
        public async Task GetAllUserPoints_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow
            };

            await _userPointRepository.Add(userPoint);

            var result = await _userPointRepository.Get();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test]
        public async Task UpdateUserPoint_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow
            };

            await _userPointRepository.Add(userPoint);

            // Update points
            userPoint.Points = 200;

            var result = await _userPointRepository.Update(userPoint);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(200, result.Points);
        }

        [Test]
        public async Task DeleteUserPoint_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow
            };

            await _userPointRepository.Add(userPoint);

            var result = await _userPointRepository.Delete(userPoint.UserPointsId);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(100, result.Points);
        }

        [Test]
        public void DeleteUserPoint_Fail()
        {
            Assert.ThrowsAsync<Exception>(async () => await _userPointRepository.Delete(999));
        }

        [Test]
        public async Task GetUserPointsByUserId_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow
            };

            await _userPointRepository.Add(userPoint);

            var result = await _userPointRepository.GetUserPointsByUserId(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(100, result.Points);
        }

        [Test]
        public void GetUserPointsByUserId_Fail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _userPointRepository.GetUserPointsByUserId(999));
        }

        [Test]
        public async Task DeductPoints_Success()
        {
            var user = await _context.Users.FirstAsync();

            var userPoint = new UserPoint
            {
                UserId = user.Id,
                Points = 100,
                LastUpdated = DateTime.UtcNow
            };

            await _userPointRepository.Add(userPoint);

            await _userPointRepository.DeductPoints(user.Id, 50);

            var result = await _userPointRepository.GetUserPointsByUserId(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(50, result.Points);
        }

        [Test]
        public void DeductPoints_Fail()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var user = await _context.Users.FirstAsync();

                var userPoint = new UserPoint
                {
                    UserId = user.Id,
                    Points = 100,
                    LastUpdated = DateTime.UtcNow
                };

                await _userPointRepository.Add(userPoint);

                await _userPointRepository.DeductPoints(user.Id, 150);
            });
        }
    }
}
