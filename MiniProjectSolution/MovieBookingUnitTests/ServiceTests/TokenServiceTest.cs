using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniProjectAPI.Models;
using Moq;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.ServiceTests
{
    public class TokenServiceTest
    {
        //private DbContextOptions<MovieBookingContext> _options;
       // private MovieBookingContext _context;
        public async Task Setup()
        {

        }
        [Test]
        public async Task TokenSuccess()
        {
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            string jwtstring = "This is my JWT signature for the Movie Booking App which has to be a bit long like 512 bytes I guess this works";
            configurationJWTSection.Setup(x => x.Value).Returns(jwtstring);
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenService tokenservice = new TokenService(mockConfig.Object);
            var user = new User()
               {
                   Email = "elonmusk@gmail.com",
                   Name = "Elon Musk",
                   DateOfBirth = new DateTime(2003,05,09),
                   Image = "elon.png",
                   Phone = "9080950106",
                   Role = "Admin",
               };
            var token = tokenservice.GenerateToken(user);
            Assert.IsNotNull(token);
        }
    }
}
