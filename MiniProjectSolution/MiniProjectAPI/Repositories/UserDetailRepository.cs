using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class UserDetailRepository : IRepository<int, UserDetail>
    {
        private readonly MovieBookingContext _context;

        public UserDetailRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<UserDetail> Add(UserDetail item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UserDetail> Delete(int key)
        {
            var userDetails = await Get(key);
            if (userDetails != null)
            {
                _context.Remove(userDetails);
                await _context.SaveChangesAsync();
                return userDetails;
            }
            throw new EntityNotFoundException("UserDetail");
        }

        public async Task<UserDetail> Get(int key)
        {
            var userDetailsList = await _context.UserDetails.ToListAsync();
            var userDetails = userDetailsList.Find(x => x.UserId == key);
            if (userDetails != null)
                return userDetails;
            throw new EntityNotFoundException("UserDetail");
        }

        public async Task<IEnumerable<UserDetail>> Get()
        {
            var result = (await _context.UserDetails.ToListAsync());
            if (result.Count == 0)
                throw new NoEntitiesFoundException("UserDetail");
            return result;
        }

        public async Task<UserDetail> Update(UserDetail item)
        {
            var userDetails = await Get(item.UserId);
            if (userDetails != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return userDetails;
            }
            throw new EntityNotFoundException("UserDetail");
        }
    }

}
