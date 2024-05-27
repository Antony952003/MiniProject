using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class BookingSnackRepository : IRepository<int, BookingSnack>
    {
        private readonly MovieBookingContext _context;

        public BookingSnackRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<BookingSnack> Add(BookingSnack item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<BookingSnack> Delete(int key)
        {
            var snack = await Get(key);
            if (snack != null)
            {
                _context.Remove(snack);
                await _context.SaveChangesAsync();
                return snack;
            }
            throw new EntityNotFoundException("BookingSnack");
        }

        public async Task<BookingSnack> Get(int key)
        {
            var snacks = await _context.BookingSnacks
                .Include(x => x.Snack)
                .ToListAsync();
            var snack = snacks.Find(x => x.SnackId == key);
            if (snack != null)
                return snack;
            throw new EntityNotFoundException("BookingSnack");
        }

        public async Task<IEnumerable<BookingSnack>> Get()
        {
            var result = await _context.BookingSnacks
                .ToListAsync();
            if (result.Count == 0)
                throw new NoEntitiesFoundException("BookingSnack");
            return result;
        }

        public async Task<BookingSnack> Update(BookingSnack item)
        {
            var snack = await Get(item.SnackId);
            if (snack != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return snack;
            }
            throw new EntityNotFoundException("BookingSnack");
        }
    }

}
