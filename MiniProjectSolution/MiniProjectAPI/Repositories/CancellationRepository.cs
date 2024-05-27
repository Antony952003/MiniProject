using Microsoft.EntityFrameworkCore;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class CancellationRepository : IRepository<int, Cancellation>
    {
        private readonly MovieBookingContext _context;

        public CancellationRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Cancellation> Add(Cancellation item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Cancellation> Delete(int key)
        {
            var cancellation = await Get(key);
            if (cancellation != null)
            {
                _context.Remove(cancellation);
                await _context.SaveChangesAsync();
                return cancellation;
            }
            throw new EntityNotFoundException("Cancellation");
        }

        public async Task<Cancellation> Get(int key)
        {
            var cancellations = await _context.Cancellations.ToListAsync();
            var cancellation = cancellations.Find(x => x.CancellationId == key);
            if (cancellation != null)
                return cancellation;
            throw new EntityNotFoundException("Cancellation");
        }

        public async Task<IEnumerable<Cancellation>> Get()
        {
            var result = (await _context.Cancellations.ToListAsync());
            if (result.Count == 0)
                throw new NoEntitiesFoundException("Cancellation");
            return result;
        }

        public async Task<Cancellation> Update(Cancellation item)
        {
            var cancellation = await Get(item.CancellationId);
            if (cancellation != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return cancellation;
            }
            throw new EntityNotFoundException("Cancellation");
        }
    }

}
