using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class ReviewRepository : IRepository<int, Review>
    {
        private readonly MovieBookingContext _context;

        public ReviewRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Review> Add(Review item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Review> Delete(int key)
        {
            var review = await Get(key);
            if (review != null)
            {
                _context.Remove(review);
                await _context.SaveChangesAsync();
                return review;
            }
            throw new EntityNotFoundException("Review");
        }

        public async Task<Review> Get(int key)
        {
            var reviews = await _context.Reviews.ToListAsync();
            var review = reviews.Find(x => x.ReviewId == key);
            if (review != null)
                return review;
            throw new EntityNotFoundException("Review");
        }

        public async Task<IEnumerable<Review>> Get()
        {
            var result = (await _context.Reviews.ToListAsync());
            if (result.Count == 0)
                throw new NoEntitiesFoundException("Review");
            return result;
        }

        public async Task<Review> Update(Review item)
        {
            var review = await Get(item.ReviewId);
            if (review != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return review;
            }
            throw new EntityNotFoundException("Review");
        }
    }

}
