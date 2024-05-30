using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models.DTOs.Seat;
using MovieBookingAPI.Models.DTOs.Theater;
using MovieBookingAPI.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class SeatServiceTest : BasicSetup
    {
        private SeatService _seatService;

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            _seatService = new SeatService(_seatRepo, _screenRepo);
        }

        [Test]
        public async Task GenerateSeatsForScreen_Success()
        {
            context.Theaters.Add(new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu"
            });
            context.SaveChanges();
            var theater = await context.Theaters.FirstAsync();
            // Arrange
            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 20,
                TheaterId = theater.TheaterId,
                
            };
            context.Screens.Add(screen);
            context.SaveChanges();
            screen = await context.Screens.FirstAsync();
            var seatGenerateDTO = new SeatGenerationInputDTO
            {
                ScreenId = screen.ScreenId,
                ColumnsPerRow = new Dictionary<string, int>
                {
                    { "A-C", 10 }
                },
                RowPrices = new Dictionary<string, decimal>
                {
                    { "A-C", 170 }
                },
            };

            // Act
            var result = await _seatService.GenerateSeatsForScreen(seatGenerateDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(30, result.Count());
            Assert.AreEqual(30, context.Seats.Count());

            var seat = result.First();
            Assert.AreEqual("A1", seat.SeatNumber);
            Assert.AreEqual(170, seat.Price);

            var updatedScreen = context.Screens.Find(screen.ScreenId);
            Assert.AreEqual(30, updatedScreen.SeatingCapacity);
        }

        [Test]
        public void GenerateSeatsForScreen_ScreenNotFound()
        {
            // Arrange
            var seatGenerateDTO = new SeatGenerationInputDTO
            {
                ScreenId = 999, // non-existent screen ID
                ColumnsPerRow = new Dictionary<string, int>
                {
                    { "A-C", 10 }
                },
                RowPrices = new Dictionary<string, decimal>
                {
                    { "A-C", 12.50m }
                }
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _seatService.GenerateSeatsForScreen(seatGenerateDTO));
        }

        [Test]
        public async Task GenerateSeatsForScreen_ValidateSeats()
        {
            // Arrange
            context.Theaters.Add(new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu"
            });
            context.SaveChanges();
            var theater = await context.Theaters.FirstAsync();
            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 20,
                TheaterId = theater.TheaterId
            };
            context.Screens.Add(screen);
            context.SaveChanges();
            screen = await context.Screens.FirstAsync();

            var seatGenerateDTO = new SeatGenerationInputDTO
            {
                ScreenId = screen.ScreenId,
                ColumnsPerRow = new Dictionary<string, int>
                {
                    { "A-B", 5 }
                },
                RowPrices = new Dictionary<string, decimal>
                {
                    { "A-B", 10.00m },
                }
            };

            // Act
            var result = await _seatService.GenerateSeatsForScreen(seatGenerateDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Count());

            var seatA1 = result.First(s => s.SeatNumber == "A1");
            var seatB1 = result.First(s => s.SeatNumber == "B1");

            Assert.AreEqual(10.00m, seatA1.Price);
            Assert.AreEqual(10.00m, seatB1.Price);

            var updatedScreen = context.Screens.Find(screen.ScreenId);
            Assert.AreEqual(10, updatedScreen.SeatingCapacity);
        }
    }
}
