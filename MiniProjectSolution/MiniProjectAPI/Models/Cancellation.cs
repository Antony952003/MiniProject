using MiniProjectAPI.Models;

namespace MovieBookingAPI.Models
{
    public class Cancellation
    {
        public int CancellationId { get; set; }
        public int BookingId { get; set; }
        public DateTime CancellationDate { get; set; }
        public decimal RefundAmount { get; set; }
        public Booking Booking { get; set; }
        public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; }
    }
}
