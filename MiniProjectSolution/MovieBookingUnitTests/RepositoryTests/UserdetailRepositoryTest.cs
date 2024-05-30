using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Repositories;
using NUnit.Framework;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class UserDetailRepositoryTests
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private UserDetailRepository _userDetailRepository;

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
                DateOfBirth = new DateTime(2003,05,09,00,00,00),
                Image = "elon.png",
                Phone = "9080950106",
                Role = "Admin",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _userDetailRepository = new UserDetailRepository(_context);
            
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        private void GenerateHashedPassword(out byte[] hashedPassword, out byte[] hashedKey, string password)
        {
            using (HMACSHA512 hMACSHA = new HMACSHA512())
            {
                hashedKey = hMACSHA.Key;
                hashedPassword = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        [Test]
        public async Task AddUserDetail_Success()
        {
            var user = await _context.Users.FirstAsync();
            byte[] hashedPassword;
            byte[] hashedKey;
            string password = "somethingdumb";
            GenerateHashedPassword(out hashedPassword, out hashedKey, password);

            var userDetail = new UserDetail
            {
                UserId = user.Id,
                Password = hashedPassword,
                PasswordHashKey = hashedKey,
            };

            var result = await _userDetailRepository.Add(userDetail);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDetail.User.Id, result.UserId);
            Assert.AreEqual(hashedPassword, result.Password);
            Assert.AreEqual(hashedKey, result.PasswordHashKey);
        }

        [Test]
        public async Task GetUserDetailById_Success()
        {
            var user = await _context.Users.FirstAsync();
            byte[] hashedPassword;
            byte[] hashedKey;
            string password = "somethingdumb";
            GenerateHashedPassword(out hashedPassword, out hashedKey, password);

            var userDetail = new UserDetail
            {
                UserId = user.Id,
                Password = hashedPassword,
                PasswordHashKey = hashedKey,
            };

            await _userDetailRepository.Add(userDetail);

            var result = await _userDetailRepository.Get(userDetail.UserId);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(hashedPassword, result.Password);
            Assert.AreEqual(hashedKey, result.PasswordHashKey);
        }

        [Test]
        public void GetUserDetailById_Fail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _userDetailRepository.Get(999));
        }

        [Test]
        public async Task GetAllUserDetails_Success()
        {
            var user = await _context.Users.FirstAsync();
            byte[] hashedPassword;
            byte[] hashedKey;
            string password = "somethingdumb";
            GenerateHashedPassword(out hashedPassword, out hashedKey, password);

            var userDetail = new UserDetail
            {
                UserId = user.Id,
                Password = hashedPassword,
                PasswordHashKey = hashedKey,
            };

            await _userDetailRepository.Add(userDetail);

            var result = await _userDetailRepository.Get();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test]
        public async Task UpdateUserDetail_Success()
        {
            var user = await _context.Users.FirstAsync();
            byte[] hashedPassword;
            byte[] hashedKey;
            string password = "somethingdumb";
            GenerateHashedPassword(out hashedPassword, out hashedKey, password);

            var userDetail = new UserDetail
            {
                UserId = user.Id,
                Password = hashedPassword,
                PasswordHashKey = hashedKey,
            };

            await _userDetailRepository.Add(userDetail);

            // Update password
            string newPassword = "newpassword";
            GenerateHashedPassword(out hashedPassword, out hashedKey, newPassword);
            userDetail.Password = hashedPassword;
            userDetail.PasswordHashKey = hashedKey;

            var result = await _userDetailRepository.Update(userDetail);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(hashedPassword, result.Password);
            Assert.AreEqual(hashedKey, result.PasswordHashKey);
        }

        [Test]
        public async Task DeleteUserDetail_Success()
        {
            var user = await _context.Users.FirstAsync();
            byte[] hashedPassword;
            byte[] hashedKey;
            string password = "somethingdumb";
            GenerateHashedPassword(out hashedPassword, out hashedKey, password);

            var userDetail = new UserDetail
            {
                UserId = user.Id,
                Password = hashedPassword,
                PasswordHashKey = hashedKey,
            };

            await _userDetailRepository.Add(userDetail);

            var result = await _userDetailRepository.Delete(userDetail.UserId);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.UserId);
            Assert.AreEqual(hashedPassword, result.Password);
            Assert.AreEqual(hashedKey, result.PasswordHashKey);
        }

        [Test]
        public void DeleteUserDetail_Fail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _userDetailRepository.Delete(999));
        }

        [Test]
        public void GetAllUserDetails_Fail()
        {
            Assert.ThrowsAsync<NoEntitiesFoundException>(async () => await _userDetailRepository.Get());
        }
    }
}
