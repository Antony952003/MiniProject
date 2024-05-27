namespace MiniProjectAPI.Models
{
    public class BookingSnack
    {
        public int BookingSnackId { get; set; }
        public int? BookingId { get; set; }
        public int SnackId { get; set; }
        public int Quantity { get; set; }
        public Booking Booking { get; set; }
        public Snack Snack { get; set; }
    }
}