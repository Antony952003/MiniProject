using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class ShowtimeSeatRepository : IRepository<int, ShowtimeSeat>
    {
        private readonly MovieBookingContext _context;
        public ShowtimeSeatRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<ShowtimeSeat> Add(ShowtimeSeat item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<ShowtimeSeat> Delete(int key)
        {
            var showtimeseat = await Get(key);
            if (showtimeseat != null)
            {
                _context.Remove(showtimeseat);
                await _context.SaveChangesAsync();
                return showtimeseat;
            }
            throw new EntityNotFoundException("ShowtimeSeat");
        }

        public async Task<ShowtimeSeat> Get(int key)
        {
            var showtimeseats = await _context.ShowtimeSeats.ToListAsync();
            var showtimeseat = showtimeseats.Find(x => x.SeatId == key);
            if (showtimeseat != null)
                return showtimeseat;
            return null;
        }

        public async Task<IEnumerable<ShowtimeSeat>> Get()
        {
            var result = await _context.ShowtimeSeats
                .Include(x => x.Showtime)
                .Include(x => x.Seat)
                .Include(x => x.Showtime)
                .ToListAsync();
            if (result.Count == 0)
                throw new NoEntitiesFoundException("ShowtimeSeat");
            return result;
        }

        public async Task<ShowtimeSeat> Update(ShowtimeSeat item)
        {
            var showtimeseat = await Get(item.SeatId);
            if (showtimeseat != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return showtimeseat;
            }
            throw new EntityNotFoundException("ShowtimeSeat");
        }
    }
}
