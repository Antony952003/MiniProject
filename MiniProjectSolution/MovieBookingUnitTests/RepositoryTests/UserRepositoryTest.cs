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
    public class UserRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private UserRepository _userRepository;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _userRepository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddUserSuccess()
        {
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106"
            };

            var result = await _userRepository.Add(user);

            Assert.IsNotNull(result);
            Assert.AreEqual("Elon Musk", result.Name);
        }

        [Test]
        public void AddUser_NullUser_ShouldThrowException_Fail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _userRepository.Add(null));
        }

        [Test]
        public async Task GetUserByIdSuccess()
        {
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106"
            };
            user = await _userRepository.Add(user);

            var result = await _userRepository.Get(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
        }

        [Test]
        public async Task GetUserByIdFail()
        {
            Assert.IsNull(await _userRepository.Get(999));
        }

        [Test]
        public async Task GetAllUsersSuccess()
        {
            var user1 = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106"
            };
            var user2 = new User
            {
                Name = "Mark Zuckerberg",
                Email = "markmaama@gmail.com",
                DateOfBirth = new DateTime(1995, 5, 5),
                Phone = "9876543210"
            };
            await _userRepository.Add(user1);
            await _userRepository.Add(user2);

            var result = await _userRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllUsers_Success()
        {
            var result = await _userRepository.Get();
            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateUserSuccess()
        {
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106"
            };
            user = await _userRepository.Add(user);
            user.Name = "ElonMusk";
            user.Phone = "9962626266";

            var result = await _userRepository.Update(user);

            Assert.IsNotNull(result);
            Assert.AreEqual("ElonMusk", result.Name);
            Assert.AreEqual("9962626266", result.Phone);
        }

        [Test]
        public void UpdateUserFail()
        {
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106"
            };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _userRepository.Update(user));
        }

        [Test]
        public async Task DeleteUserSuccess()
        {
            var user = new User
            {
                Name = "Elon Musk",
                Email = "elonmusk@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "9080950106"
            };
            user = await _userRepository.Add(user);

            var result = await _userRepository.Delete(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
        }

        [Test]
        public void DeleteUserFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _userRepository.Delete(999));
        }
    }
}
