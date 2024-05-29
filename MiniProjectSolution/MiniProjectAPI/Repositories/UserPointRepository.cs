using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class UserPointRepository : IUserPointRepository
    {
        private readonly MovieBookingContext _context;

        public UserPointRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<UserPoint> Add(UserPoint item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UserPoint> Delete(int key)
        {
            var userPoint = await _context.UserPoints.FindAsync(key);
            if (userPoint != null)
            {
                _context.UserPoints.Remove(userPoint);
                await _context.SaveChangesAsync();
                return userPoint;
            }
            throw new Exception("UserPoint not found");
        }

        public async Task<UserPoint> Update(UserPoint item)
        {
            var userPoint = await Get(item.UserPointsId);
            if (userPoint != null)
            {
                _context.Update(userPoint);
                await _context.SaveChangesAsync();
                return userPoint;
            }
            throw new EntityNotFoundException("UserPoint");
        }

        public async Task<UserPoint> Get(int key)
        {
            var userpoints = await Get();
            var userpoint = userpoints.ToList().FirstOrDefault(x => x.UserPointsId == key);
            if(userpoint  != null)
                return userpoint;
            return null;
        }

        public async Task<IEnumerable<UserPoint>> Get()
        {
            var userpoints = await _context.UserPoints
                .Include(x => x.User)
                .ToListAsync();
            if(userpoints != null)
                return userpoints;
            return null;
        }

        public async Task<UserPoint> GetUserPointsByUserId(int userId)
        {
            var userpoints = await Get();
            var userpointsofuser = userpoints.ToList().Find(x => x.UserId == userId);
            if (userpointsofuser != null)
                return userpointsofuser;
            throw new EntityNotFoundException("UserPoint");
        }
        public async Task DeductPoints(int userId, int points)
        {
            var userpoint = await GetUserPointsByUserId(userId);
            userpoint.Points -= points;

            if (userpoint.Points < 0)
            {
                throw new InvalidOperationException("User does not have enough points to deduct");
            }
        }
    }
}
