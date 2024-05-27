using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class BookingRepository : IRepository<int, Booking>
    {
        private readonly MovieBookingContext _context;

        public BookingRepository(MovieBookingContext context)
        {
            _context = context;
        }
        public async Task<Booking> Add(Booking item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Booking> Delete(int key)
        {
            var booking = await Get(key);
            if (booking != null)
            {
                _context.Remove(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            throw new EntityNotFoundException("Booking");
        }

        public async Task<Booking> Get(int key)
        {
            var bookings = await Get();
            var booking = bookings.ToList().Find(x => x.BookingId == key);
            if (booking != null)
                return booking;
            return null;
        }

        public async Task<IEnumerable<Booking>> Get()
        {
            var result = (await _context.Bookings
                .Include(x => x.Showtime)
                .Include(x => x.User)
                .Include(x => x.BookingSnacks)
                .Include(x => x.ShowtimeSeats)
                .Include(x => x.Cancellations)
                .ToListAsync());
            if (result.Count == 0)
                return null;
            return result;
        }

        public async Task<Booking> Update(Booking item)
        {
            var booking = await Get(item.BookingId);
            if (booking != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return booking;
            }
            throw new EntityNotFoundException("Booking");
        }
    }
}
