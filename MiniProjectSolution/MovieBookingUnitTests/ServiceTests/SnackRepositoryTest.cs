using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models.DTOs.Snack;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class SnackServiceTest : BasicSetup
    {
        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task AddSnack_Success()
        {
            // Arrange
            var snackInputDTO = new SnackInputDTO
            {
                Name = "Popcorn",
                Price = 5.50m
            };

            // Act
            var result = await _snackService.AddSnack(snackInputDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(snackInputDTO.Name, result.Name);
            Assert.AreEqual(snackInputDTO.Price, result.Price);
        }

        [Test]
        public async Task GetSnackIdByName_Success()
        {
            // Arrange
            var snack = new Snack
            {
                Name = "Popcorn",
                Price = 5.50m
            };
            context.Snacks.Add(snack);
            context.SaveChanges();

            // Act
            var result = await _snackService.GetSnackIdByName(snack.Name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(snack.Name, result.Name);
        }

        [Test]
        public void GetSnackIdByName_Failure()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _snackService.GetSnackIdByName("Nonexistent Snack"));
        }

        [Test]
        public async Task UpdateSnackPrice_Success()
        {
            // Arrange
            var snack = new Snack
            {
                Name = "Popcorn",
                Price = 5.50m
            };
            context.Snacks.Add(snack);
            context.SaveChanges();

            var newPrice = 6.00m;

            // Act
            var result = await _snackService.UpdateSnackPrice(snack.Name, newPrice);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(snack.Name, result.Name);
            Assert.AreEqual(newPrice, result.Price);
        }

        [Test]
        public void UpdateSnackPrice_Failure()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _snackService.UpdateSnackPrice("Nonexistent Snack", 6.00m));
        }
    }
}
