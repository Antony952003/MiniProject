namespace MovieBookingAPI.Models.DTOs.Booking
{
    public class BookingReturnDTO
    {
        public int BookingId { get; set; }
        public string TheaterName { get; set; }
        public string MovieName { get; set; }
        public string ScreenName { get; set; }
        public List<string> BookedTickets { get; set; }
        public Dictionary<string, int>? SnacksOrderedWithTickets { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
    }
}
