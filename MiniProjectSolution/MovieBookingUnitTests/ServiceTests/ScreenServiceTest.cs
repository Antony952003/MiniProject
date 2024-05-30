using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models.DTOs.Screen;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MovieBookingUnitTests.ServiceTests
{
    public class ScreenServiceTest : BasicSetup
    {
        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task AddScreenToTheater_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Test Theater", Location = "Test Location" };
            context.Theaters.Add(theater);
            context.SaveChanges();

            var screenDTO = new ScreenDTO
            {
                ScreenId = 1,
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId
            };

            // Act
            var result = await _screenService.AddScreenToTheater(screenDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(screenDTO.Name, result.ScreenName);
            Assert.AreEqual(screenDTO.SeatingCapacity, result.SeatingCapacity);
            Assert.AreEqual(theater.Name, result.TheaterName);
        }

        [Test]
        public void AddScreenToTheater_Failure_ScreenAlreadyExists()
        {
            // Arrange
            var theater = new Theater { Name = "Test Theater", Location = "Test Location" };
            context.Theaters.Add(theater);
            context.SaveChanges();

            var screen = new Screen
            {
                ScreenId = 1,
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId
            };
            context.Screens.Add(screen);
            context.SaveChanges();

            var screenDTO = new ScreenDTO
            {
                ScreenId = 1,
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId
            };

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExists>(() => _screenService.AddScreenToTheater(screenDTO));
        }

        [Test]
        public async Task GetScreensByTheaterName_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Test Theater", Location = "Test Location" };
            context.Theaters.Add(theater);
            context.SaveChanges();

            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId
            };
            context.Screens.Add(screen);
            context.SaveChanges();

            // Act
            var result = await _screenService.GetScreensByTheaterName(theater.Name);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(screen.Name, result.First().ScreenName);
            Assert.AreEqual(screen.SeatingCapacity, result.First().SeatingCapacity);
            Assert.AreEqual(theater.Name, result.First().TheaterName);
        }

        [Test]
        public void GetScreensByTheaterName_Failure_NoScreensFound()
        {
            // Arrange
            var theaterName = "Non-existent Theater";

            // Act & Assert
            Assert.ThrowsAsync<NoEntitiesFoundException>(() => _screenService.GetScreensByTheaterName(theaterName));
        }

        [Test]
        public async Task GetScreenByScreenName_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Test Theater", Location = "Test Location" };
            context.Theaters.Add(theater);
            context.SaveChanges();
            theater = await context.Theaters.FirstAsync();
            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId
            };
            context.Screens.Add(screen);
            context.SaveChanges();

            // Act
            var result = await _screenService.GetScreenByScreenName(screen.Name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(screen.Name, result.ScreenName);
            Assert.AreEqual(screen.SeatingCapacity, result.SeatingCapacity);
            Assert.AreEqual(theater.Name, result.TheaterName);
        }

        [Test]
        public void GetScreenByScreenName_Failure_ScreenNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<NoEntitiesFoundException>(() => _screenService.GetScreenByScreenName("Non-existent Screen"));
        }

        [Test]
        public async Task GetScreenByScreenId_Success()
        {
            // Arrange
            var theater = new Theater { Name = "Test Theater", Location = "Test Location" };
            context.Theaters.Add(theater);
            context.SaveChanges();

            var screen = new Screen
            {
                Name = "Screen 1",
                SeatingCapacity = 100,
                TheaterId = theater.TheaterId
            };
            context.Screens.Add(screen);
            context.SaveChanges();

            // Act
            var result = await _screenService.GetScreenByScreenId(screen.ScreenId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(screen.Name, result.ScreenName);
            Assert.AreEqual(screen.SeatingCapacity, result.SeatingCapacity);
            Assert.AreEqual(theater.Name, result.TheaterName);
        }

        [Test]
        public void GetScreenByScreenId_Failure_ScreenNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _screenService.GetScreenByScreenId(1));
        }
    }
}
