using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class SeatRepository : IRepository<int, Seat>
    {
        private readonly MovieBookingContext _context;

        public SeatRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Seat> Add(Seat item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Seat> Delete(int key)
        {
            var seat = await Get(key);
            if (seat != null)
            {
                _context.Remove(seat);
                await _context.SaveChangesAsync();
                return seat;
            }
            throw new EntityNotFoundException("Seat");
        }

        public async Task<Seat> Get(int key)
        {
            var seats = await Get();
            var seat = seats.ToList().Find(x => x.SeatId == key);
            if (seat != null)
                return seat;
            throw new EntityNotFoundException("Seat");
        }

        public async Task<IEnumerable<Seat>> Get()
        {
            var result = await _context.Seats
                .Include(x => x.ShowtimeSeats)
                .ToListAsync();
            if (result.Count == 0)
                throw new NoEntitiesFoundException("Seat");
            return result;
        }

        public async Task<Seat> Update(Seat item)
        {
            var seat = await Get(item.SeatId);
            if (seat != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return seat;
            }
            throw new EntityNotFoundException("Seat");
        }
    }

}
