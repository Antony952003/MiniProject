using MovieBookingAPI.Contexts;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class TransactionRepository : ITransaction
    {
        private readonly MovieBookingContext _context;

        public TransactionRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
