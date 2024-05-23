using MovieBookingAPI.Models;

namespace MiniProjectAPI.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int ScreenId { get; set; }
        public string SeatNumber { get; set; }
        public string Row { get; set; }
        public int Column { get; set; }
        public decimal price { get; set; }
        public bool IsBooked { get; set; }
        public bool IsBlocked { get; set; }
        public int BookingId { get; set; }
        public int? CancellationId { get; set; }
        public Screen Screen { get; set; }
        public Booking Booking { get; set; }
        public Cancellation Cancellation { get; set; }
    }
}