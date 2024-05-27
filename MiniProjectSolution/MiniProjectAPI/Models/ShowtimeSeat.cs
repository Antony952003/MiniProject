using MiniProjectAPI.Models;

namespace MovieBookingAPI.Models
{
    public class ShowtimeSeat
    {
        public int ShowtimeSeatId { get; set; }
        public string Status { get; set; }
        public int SeatId { get; set; }
        public Seat Seat { get; set; }
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; }
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public int? CancellationId { get; set; }
        public Cancellation? Cancellation { get; set; }
    }
}
