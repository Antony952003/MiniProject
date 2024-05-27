using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class MovieRepository : IRepository<int, Movie>
    {
        private readonly MovieBookingContext _context;

        public MovieRepository(MovieBookingContext context) {
            _context = context;
        }
        public async Task<Movie> Add(Movie item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Movie> Delete(int key)
        {
            var movie = await Get(key);
            if (movie != null)
            {
                _context.Remove(movie);
                await _context.SaveChangesAsync();
                return movie;
            }
            throw new EntityNotFoundException("Movie");
        }

        public async Task<Movie> Get(int key)
        {
            var movies = await _context.Movies.ToListAsync();
            var movie = movies.Find(x => x.MovieId == key);
            if (movie != null)
                return movie;
            return null;
        }

        public async Task<IEnumerable<Movie>> Get()
        {
            var result = (await _context.Movies.ToListAsync());
            if (result.Count == 0)
                return null;
            return result;
        }

        public async Task<Movie> Update(Movie item)
        {
            var movie = await Get(item.MovieId);
            if (movie != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return movie;
            }
            throw new EntityNotFoundException("Movie");
        }
    }
}
