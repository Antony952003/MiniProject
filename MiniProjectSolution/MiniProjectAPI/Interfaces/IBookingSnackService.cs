using MiniProjectAPI.Models;

namespace MovieBookingAPI.Interfaces
{
    public interface IBookingSnackService
    {
        public Task<BookingSnack> SnackBooking(Snack snack, int quantity, int bookingId);
    }
}
