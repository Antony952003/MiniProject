using MovieBookingAPI.Models;

namespace MiniProjectAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public decimal TotalPrice { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; } //pending, successfull
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public User User { get; set; }
        public Showtime Showtime { get; set; }
        public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; }
        public ICollection<BookingSnack> BookingSnacks { get; set; }
        public ICollection<Cancellation> Cancellations { get; set; }

        public Payment Payment { get; set; }

    }
}