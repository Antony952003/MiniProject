using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class SnackRepository : IRepository<int, Snack>
    {
        private readonly MovieBookingContext _context;

        public SnackRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Snack> Add(Snack item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Snack> Delete(int key)
        {
            var snack = await Get(key);
            if (snack != null)
            {
                _context.Remove(snack);
                await _context.SaveChangesAsync();
                return snack;
            }
            throw new EntityNotFoundException("Snack");
        }

        public async Task<Snack> Get(int key)
        {
            var snacks = await _context.Snacks.ToListAsync();
            var snack = snacks.Find(x => x.SnackId == key);
            if (snack != null)
                return snack;
            throw new NoEntitiesFoundException("Snack");
        }

        public async Task<IEnumerable<Snack>> Get()
        {
            var result = (await _context.Snacks
                .Include(x =>x.BookingSnacks)
                .ToListAsync());
            if (result.Count == 0)
                throw new EntityNotFoundException("Snack");
            return result;
        }

        public async Task<Snack> Update(Snack item)
        {
            var snack = await Get(item.SnackId);
            if (snack != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return snack;
            }
            throw new EntityNotFoundException("Snack");
        }
    }

}
