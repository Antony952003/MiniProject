using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Repositories
{
    public class PaymentRepository : IRepository<int, Payment>
    {
        private readonly MovieBookingContext _context;

        public PaymentRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<Payment> Add(Payment item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Payment> Delete(int key)
        {
            var payment = await Get(key);
            if (payment != null)
            {
                _context.Remove(payment);
                await _context.SaveChangesAsync();
                return payment;
            }
            throw new EntityNotFoundException("Payment");
        }

        public async Task<Payment> Get(int key)
        {
            var payments = await _context.Payments.ToListAsync();
            var payment = payments.Find(x => x.PaymentId == key);
            if (payment != null)
                return payment;
            throw new EntityNotFoundException("Payment");
        }

        public async Task<IEnumerable<Payment>> Get()
        {
            var result = (await _context.Payments.ToListAsync());
            if (result.Count == 0)
                throw new NoEntitiesFoundException("Payment");
            return result;
        }

        public async Task<Payment> Update(Payment item)
        {
            var payment = await Get(item.PaymentId);
            if (payment != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return payment;
            }
            throw new EntityNotFoundException("Payment");
        }
    }

}
