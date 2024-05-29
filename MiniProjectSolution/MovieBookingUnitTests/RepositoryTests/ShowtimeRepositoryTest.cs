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
    public class ShowtimeRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private ShowtimeRepository _showtimeRepository;
        private MovieRepository _movieRepository;
        private ScreenRepository _screenRepository;
        private TheaterRepository _theaterRepository;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _showtimeRepository = new ShowtimeRepository(_context);
            _movieRepository = new MovieRepository(_context);
            _screenRepository = new ScreenRepository(_context);
            _theaterRepository = new TheaterRepository(_context);

            var movie = new Movie
            {
                Title = "Inception",
                Description = "A mind-bending thriller",
                Cast = "Leonardo DiCaprio",
                Director = "Christopher Nolan",
                DurationInHours = 2.5,
                Genre = "Sci-Fi"
            };
            await _movieRepository.Add(movie);

            var theater = new Theater
            {
                Name = "Rohini Cinemas",
                Location = "Vadapalani"
            };
            await _theaterRepository.Add(theater);

            var screen = new Screen
            {
                Name = "PXL",
                SeatingCapacity = 150,
                TheaterId = theater.TheaterId
            };
            await _screenRepository.Add(screen);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddShowtimeSuccess()
        {
            var movie = await _context.Movies.FirstAsync();
            var screen = await _context.Screens.FirstAsync();
            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now
            };

            var result = await _showtimeRepository.Add(showtime);

            Assert.IsNotNull(result);
            Assert.AreEqual(movie.MovieId, result.MovieId);
        }

        [Test]
        public void AddShowtimeFail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _showtimeRepository.Add(null));
        }

        [Test]
        public async Task GetShowtimeByIdSuccess()
        {
            var movie = await _context.Movies.FirstAsync();
            var screen = await _context.Screens.FirstAsync();
            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now
            };
            showtime = await _showtimeRepository.Add(showtime);

            var result = await _showtimeRepository.Get(showtime.ShowtimeId);

            Assert.IsNotNull(result);
            Assert.AreEqual(showtime.ShowtimeId, result.ShowtimeId);
        }

        [Test]
        public void GetShowtimeByIdFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _showtimeRepository.Get(999));
        }

        [Test]
        public async Task GetAllShowtimesSuccess()
        {
            var movie = await _context.Movies.FirstAsync();
            var screen = await _context.Screens.FirstAsync();
            var showtime1 = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now
            };
            var showtime2 = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now.AddHours(3)
            };
            await _showtimeRepository.Add(showtime1);
            await _showtimeRepository.Add(showtime2);

            var result = await _showtimeRepository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllShowtimes_Fail()
        {
            Assert.ThrowsAsync<NoEntitiesFoundException>(async () => await _showtimeRepository.Get());
        }

        [Test]
        public async Task UpdateShowtimeSuccess()
        {
            var movie = await _context.Movies.FirstAsync();
            var screen = await _context.Screens.FirstAsync();
            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now
            };
            showtime = await _showtimeRepository.Add(showtime);
            showtime.StartTime = DateTime.Now.AddHours(1);

            var result = await _showtimeRepository.Update(showtime);

            Assert.IsNotNull(result);
            Assert.AreEqual(showtime.StartTime, result.StartTime);
        }

        [Test]
        public void UpdateShowtimeFail()
        {
            var showtime = new Showtime
            {
                ShowtimeId = 999,
                MovieId = 1,
                ScreenId = 1,
                StartTime = DateTime.Now
            };

            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _showtimeRepository.Update(showtime));
        }

        [Test]
        public async Task DeleteShowtimeSuccess()
        {
            var movie = await _context.Movies.FirstAsync();
            var screen = await _context.Screens.FirstAsync();
            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Now
            };
            showtime = await _showtimeRepository.Add(showtime);

            var result = await _showtimeRepository.Delete(showtime.ShowtimeId);

            Assert.IsNotNull(result);
            Assert.AreEqual(showtime.ShowtimeId, result.ShowtimeId);
        }

        [Test]
        public void DeleteShowtimeFail()
        {
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _showtimeRepository.Delete(999));
        }
    }
}
