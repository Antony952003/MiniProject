using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class ScreenRepository : IRepository<int, Screen>
    {
        private readonly MovieBookingContext _context;

        public ScreenRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Screen> Add(Screen item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Screen> Delete(int key)
        {
            var screen = await Get(key);
            if (screen != null)
            {
                _context.Remove(screen);
                await _context.SaveChangesAsync();
                return screen;
            }
            throw new EntityNotFoundException("Screen");
        }

        public async Task<Screen> Get(int key)
        {
            var screens = await Get();
            if (screens != null)
            {
                var screen = screens.ToList().Find(x => x.ScreenId == key);
                if (screen == null) return null;
                    return screen;
            }
            return null;
        }

        public async Task<IEnumerable<Screen>> Get()
        {
            var result = (await _context.Screens
                .Include(x => x.Theater)
                .ToListAsync());
            if (result.Count == 0)
                return null;
            return result;
        }

        public async Task<Screen> Update(Screen item)
        {
            var screen = await Get(item.ScreenId);
            if (screen != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return screen;
            }
            throw new EntityNotFoundException("Screen");
        }
    }

}
