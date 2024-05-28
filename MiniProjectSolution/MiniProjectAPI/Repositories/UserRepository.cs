using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class UserRepository : IRepository<int, User>
    {
        private MovieBookingContext _context;

        public UserRepository(MovieBookingContext context)
        {
            _context = context;
        }
        public async Task<User> Add(User item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<User> Delete(int key)
        {
            var user = await Get(key);
            if (user != null)
            {
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
            throw new EntityNotFoundException("User");
        }

        public async Task<User> Get(int key)
        {
            var users = await Get();
            var user = users.ToList().Find(x => x.Id == key);
            if (user != null)
                 return user;
            return null;
        }

        public async Task<IEnumerable<User>> Get()
        {
            var result =  (await _context.Users
                .Include (x => x.Bookings)
                .Include(x => x.Reviews)
                .ToListAsync());
            if (result.Count == 0)
                return null;
            return result;
        }

        public async Task<User> Update(User item)
        {
            var user = await Get(item.Id);
            if (user != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return user;
            }
            throw new EntityNotFoundException("User");
        }
    }
}
