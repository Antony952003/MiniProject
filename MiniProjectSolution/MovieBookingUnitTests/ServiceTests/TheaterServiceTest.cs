using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Theater;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class TheaterServiceTest : BasicSetup
    {
        [SetUp]
        public new void Setup()
        {
            base.Setup();
        }

        [Test]
        public async Task AddTheater_Success()
        {
            // Arrange
            var theaterDTO = new TheaterDTO
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu",
            };

            // Act
            var result = await _theaterService.AddTheater(theaterDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(theaterDTO.Name, result.Name);
            Assert.AreEqual(theaterDTO.Location, result.Location);
        }

        [Test]
        public async Task AddTheater_Failure_TheaterAlreadyExists()
        {
            // Arrange
            var theaterDTO = new TheaterDTO
            {
                Name = "Rohini Cinemas",
                Location = "Koyambedu",
            };

            // Add theater to the database
            context.Theaters.Add(new Theater
            {
                Name = theaterDTO.Name,
                Location = theaterDTO.Location
            });
            context.SaveChanges();
            var theater = await context.Theaters.FirstAsync();
            theaterDTO.TheaterId = theater.TheaterId;

            // Act & Assert
            Assert.ThrowsAsync<EntityAlreadyExists>(() => _theaterService.AddTheater(theaterDTO));
        }

        [Test]
        public async Task GetTheaterById_Success()
        {
            // Arrange
            var theaterDTO = new TheaterDTO
            {
                Name = "Test Theater",
                Location = "Test Location"
            };

            // Add theater to the database
            var theater = new Theater
            {
                Name = theaterDTO.Name,
                Location = theaterDTO.Location
            };
            context.Theaters.Add(theater);
            context.SaveChanges();

            // Act
            var result = await _theaterService.GetTheaterById(theater.TheaterId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(theaterDTO.Name, result.Name);
            Assert.AreEqual(theaterDTO.Location, result.Location);
        }

        [Test]
        public void GetTheaterById_Failure_TheaterNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<EntityNotFoundException>(() => _theaterService.GetTheaterById(1));
        }

        [Test]
        public async Task GetTheaterByName_Success()
        {
            // Arrange
            var theaterName = "Test Theater";
            var location = "Test Location";

            // Add theater to the database
            context.Theaters.Add(new Theater
            {
                Name = theaterName,
                Location = location
            });
            context.SaveChanges();

            // Act
            var result = await _theaterService.GetTheaterByName(theaterName);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}