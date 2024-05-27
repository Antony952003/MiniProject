using MiniProjectAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;

namespace MovieBookingAPI.Interfaces
{
    public interface IBookingService
    {
        public Task<BookingReturnDTO> MakeBooking(BookingInputDTO bookingInputDTO);
        public Task<List<BookingReturnDTO>> GetAllBookings();
        public Task<BookingReturnDTO> GetBookingById(int bookingid);
    }
}
