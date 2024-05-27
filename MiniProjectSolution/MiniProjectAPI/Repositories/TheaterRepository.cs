using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class TheaterRepository : IRepository<int, Theater>
    {
        private readonly MovieBookingContext _context;

        public TheaterRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Theater> Add(Theater item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Theater> Delete(int key)
        {
            var theatre = await Get(key);
            if (theatre != null)
            {
                _context.Remove(theatre);
                await _context.SaveChangesAsync();
                return theatre;
            }
            throw new EntityNotFoundException("Theater");
        }

        public async Task<Theater> Get(int key)
        {
            var theatres = await _context.Theaters.ToListAsync();
            var theatre = theatres.Find(x => x.TheaterId == key);
            if (theatre != null)
                return theatre;
            return null;
        }

        public async Task<IEnumerable<Theater>> Get()
        {
            var result = (await _context.Theaters
                .ToListAsync());
            if (result.Count == 0)
                throw new NoEntitiesFoundException("Theater");
            return result;
        }

        public async Task<Theater> Update(Theater item)
        {
            var theatre = await Get(item.TheaterId);
            if (theatre != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return theatre;
            }
            throw new EntityNotFoundException("Theater");
        }
    }

}
