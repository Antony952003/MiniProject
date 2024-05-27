using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class ShowtimeRepository : IRepository<int, Showtime>
    {
        private readonly MovieBookingContext _context;

        public ShowtimeRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Showtime> Add(Showtime item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Showtime> Delete(int key)
        {
            var showtime = await Get(key);
            if (showtime != null)
            {
                _context.Remove(showtime);
                await _context.SaveChangesAsync();
                return showtime;
            }
            throw new EntityNotFoundException("Showtime");
        }

        public async Task<Showtime> Get(int key)
        {
            var showtimes = await _context.Showtimes.ToListAsync();
            var showtime = showtimes.Find(x => x.ShowtimeId == key);
            if (showtime != null)
                return showtime;
            throw new EntityNotFoundException("Showtime");
        }

        public async Task<IEnumerable<Showtime>> Get()
        {
            var result = (await _context.Showtimes
                .Include(x => x.Screen)
                .Include(x => x.ShowtimeSeats)
                .Include(x => x.Bookings)
                .ToListAsync());
            if (result.Count == 0)
                throw new NoEntitiesFoundException("Showtime");
            return result;
        }

        public async Task<Showtime> Update(Showtime item)
        {
            var showtime = await Get(item.ShowtimeId);
            if (showtime != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return showtime;
            }
            throw new EntityNotFoundException("Showtime");
        }
    }

}
