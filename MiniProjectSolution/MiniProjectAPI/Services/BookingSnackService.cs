using MiniProjectAPI.Models;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Services
{
    public class BookingSnackService : IBookingSnackService
    {
        private readonly IRepository<int, BookingSnack> _bookingSnackRepo;

        public BookingSnackService(IRepository<int, BookingSnack> bookingSnackRepo) { 
            _bookingSnackRepo = bookingSnackRepo;
        }
        public async Task<BookingSnack> SnackBooking(Snack snack, int quantity, int bookingId)
        {
            BookingSnack bookingSnack = new BookingSnack()
            {
                BookingId = bookingId,
                Quantity = quantity,
                SnackId = snack.SnackId,
            };
            bookingSnack = await _bookingSnackRepo.Add(bookingSnack);
            return bookingSnack;

        }
    }
}
