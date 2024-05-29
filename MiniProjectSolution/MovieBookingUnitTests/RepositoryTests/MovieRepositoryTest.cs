using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class MovieRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private MovieRepository _repository;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _repository = new MovieRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.CloseConnection();
        }

        [Test]
        public async Task AddMovieSuccess()
        {
            var movie = new Movie()
            {
                 Title = "Fight Club",
                 Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an " +
                 "underground fight club with Tyler, a careless soap salesman",
                 Cast = "Edward Norton, Brad Pitt, Jared Leto",
                 Director = "David Fincher",
                 DurationInHours = 2.5,
                 Genre = "Thriller",
            };

            var result = await _repository.Add(movie);

            Assert.IsNotNull(result);
            Assert.That(movie.Title, Is.EqualTo(result.Title));
        }

        [Test]
        public void AddMovieFail()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test]
        public async Task GetMovieByIdSuccess()
        {
            var movie = new Movie()
            {
                Title = "Fight Club",
                Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an " +
                 "underground fight club with Tyler, a careless soap salesman",
                Cast = "Edward Norton, Brad Pitt, Jared Leto",
                Director = "David Fincher",
                DurationInHours = 2.5,
                Genre = "Thriller",
            };

            movie = await _repository.Add(movie);

            var result = await _repository.Get(movie.MovieId);

            Assert.IsNotNull(result);
            Assert.That(movie.Title, Is.EqualTo(result.Title));
        }

        [Test]
        public async Task GetMovieByIdFail()
        {
            var movie = new Movie()
            {
                Title = "Fight Club",
                Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an " +
                 "underground fight club with Tyler, a careless soap salesman",
                Cast = "Edward Norton, Brad Pitt, Jared Leto",
                Director = "David Fincher",
                DurationInHours = 2.5,
                Genre = "Thriller",
            };

            movie = await _repository.Add(movie);
            var result = await _repository.Get(2);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllMoviesSuccess()
        {
            var movie1 = new Movie()
            {
                Title = "Fight Club",
                Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an " +
                 "underground fight club with Tyler, a careless soap salesman",
                Cast = "Edward Norton, Brad Pitt, Jared Leto",
                Director = "David Fincher",
                DurationInHours = 2.5,
                Genre = "Thriller",
            };
            var movie2 = new Movie()
            {
                Title = "Wolf Of Wall Street",
                Description = "Introduced to life in the fast lane through stockbroking," +
                " Jordan Belfort takes a hit after a Wall Street crash. He teams up with Donnie Azoff," +
                " cheating his way to the top as his relationships slide.",
                Cast = "Leonardo Dicaprio, Matthew McConaughey, Margot Robbie",
                Director = "Martin Scorsesse",
                DurationInHours = 2.3,
                Genre = "Comedy, Thriller",
            };
            await _repository.Add(movie1);
            await _repository.Add(movie2);

            var result = await _repository.Get();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllMoviesFail()
        {

            var result = await _repository.Get();

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateMovieSuccess()
        {
            var movie = new Movie()
            {
                Title = "Wolf Of Wall Street",
                Description = "Introduced to life in the fast lane through stockbroking," +
                " Jordan Belfort takes a hit after a Wall Street crash. He teams up with Donnie Azoff," +
                " cheating his way to the top as his relationships slide.",
                Cast = "Leonardo Dicaprio, Matthew McConaughey, Margot Robbie",
                Director = "Martin Scorsesse",
                DurationInHours = 2.3,
                Genre = "Comedy, Thriller",
            };
            await _repository.Add(movie);
            movie.Title = "The Wolf Of Wall Street";

            var result = await _repository.Update(movie);

            Assert.IsNotNull(result);
            Assert.AreEqual(movie.Title, result.Title);
        }

        [Test]
        public async Task UpdateMovieFail()
        {
            var movie = new Movie()
            {
                Title = "Wolf Of Wall Street",
                Description = "Introduced to life in the fast lane through stockbroking," +
                " Jordan Belfort takes a hit after a Wall Street crash. He teams up with Donnie Azoff," +
                " cheating his way to the top as his relationships slide.",
                Cast = "Leonardo Dicaprio, Matthew McConaughey, Margot Robbie",
                Director = "Martin Scorsesse",
                DurationInHours = 2.3,
                Genre = "Comedy, Thriller",
            };
            var movie1 = new Movie()
            {
                MovieId = 5,
                Title = "Fight Club",
                Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an " +
                 "underground fight club with Tyler, a careless soap salesman",
                Cast = "Edward Norton, Brad Pitt, Jared Leto",
                Director = "David Fincher",
                DurationInHours = 2.5,
                Genre = "Thriller",
            };
            await _repository.Add(movie);
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.Update(movie1));
        }

        [Test]
        public async Task DeleteMovieSuccess()
        {
            var movie = new Movie()
            {
                Title = "Wolf Of Wall Street",
                Description = "Introduced to life in the fast lane through stockbroking," +
                " Jordan Belfort takes a hit after a Wall Street crash. He teams up with Donnie Azoff," +
                " cheating his way to the top as his relationships slide.",
                Cast = "Leonardo Dicaprio, Matthew McConaughey, Margot Robbie",
                Director = "Martin Scorsesse",
                DurationInHours = 2.3,
                Genre = "Comedy, Thriller",
            };
            movie = await _repository.Add(movie);

            var result = await _repository.Delete(movie.MovieId);

            Assert.IsNotNull(result);
            Assert.AreEqual(movie.Title, result.Title);
        }

        [Test]
        public async Task DeleteMovieFail()
        {
            var movie = new Movie()
            {
                Title = "Wolf Of Wall Street",
                Description = "Introduced to life in the fast lane through stockbroking," +
                " Jordan Belfort takes a hit after a Wall Street crash. He teams up with Donnie Azoff," +
                " cheating his way to the top as his relationships slide.",
                Cast = "Leonardo Dicaprio, Matthew McConaughey, Margot Robbie",
                Director = "Martin Scorsesse",
                DurationInHours = 2.3,
                Genre = "Comedy, Thriller",
            };
            await _repository.Add(movie);
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.Delete(2));
        }
    }

}
