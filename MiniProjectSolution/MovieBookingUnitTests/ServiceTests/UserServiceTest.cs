using MiniProjectAPI.Models;
using Moq;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Login;
using MovieBookingAPI.Models.DTOs.Register;
using MovieBookingAPI.Services;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class UserServiceTest
    {
        private Mock<IRepository<int, User>> _mockUserRepo;
        private Mock<IRepository<int, UserDetail>> _mockUserDetailsRepo;
        private Mock<ITokenService> _mockTokenService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IRepository<int, User>>();
            _mockUserDetailsRepo = new Mock<IRepository<int, UserDetail>>();
            _mockTokenService = new Mock<ITokenService>();
        }

        

        [Test]
        public void Login_Failure_InvalidUserId()
        {
            // Arrange
            var userService = new UserService(_mockUserDetailsRepo.Object, _mockUserRepo.Object, _mockTokenService.Object);
            var loginDTO = new UserLoginDTO
            {
                UserId = 1,
                Password = "testpassword"
            };

            _mockUserDetailsRepo.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync((UserDetail)null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => userService.Login(loginDTO));
        }

        [Test]
        public void LoginInvalidPassword()
        {
            // Arrange
            var userService = new UserService(_mockUserDetailsRepo.Object, _mockUserRepo.Object, _mockTokenService.Object);
            var loginDTO = new UserLoginDTO
            {
                UserId = 1,
                Password = "invalidpassword"
            };
            var userDetail = new UserDetail
            {
                UserId = 1,
                PasswordHashKey = Encoding.UTF8.GetBytes("testkey"),
                Password = Encoding.UTF8.GetBytes("testpassword")
            };

            _mockUserDetailsRepo.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(userDetail);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedUserException>(() => userService.Login(loginDTO));
        }

        [Test]
        public async Task Register_Success()
        {
            // Arrange
            var userService = new UserService(_mockUserDetailsRepo.Object, _mockUserRepo.Object, _mockTokenService.Object);
            var registerDTO = new RegisterInputDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "1234567890",
                Image = "test.png",
                Role = "User",
                Password = "testpassword",
                ConfirmPassword = "testpassword"
            };
            var user = new User
            {
                Id = 1,
                Name = registerDTO.Name,
                Email = registerDTO.Email,
                DateOfBirth = registerDTO.DateOfBirth,
                Phone = registerDTO.Phone,
                Image = registerDTO.Image,
                Role = registerDTO.Role
            };
            var userDetail = new UserDetail
            {
                UserId = 1,
                PasswordHashKey = Encoding.UTF8.GetBytes("testkey"),
                Password = Encoding.UTF8.GetBytes("testpassword")
            };

            _mockUserRepo.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(user);
            _mockUserDetailsRepo.Setup(repo => repo.Add(It.IsAny<UserDetail>())).ReturnsAsync(userDetail);
            _mockTokenService.Setup(service => service.GenerateToken(It.IsAny<User>())).Returns("testtoken");

            // Act
            var result = await userService.Register(registerDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.Name, result.Name);
            Assert.AreEqual(user.DateOfBirth, result.DateOfBirth);
            Assert.AreEqual(user.Phone, result.Phone);
            Assert.AreEqual(user.Image, result.Image);
            Assert.AreEqual(user.Role, result.Role);
        }

        // Add more tests for different failure scenarios (e.g., passwords don't match, unable to add user, etc.)
    }
}
