namespace MovieBookingAPI.Models.DTOs.Cancellation
{
    public class CancellationInputDTO
    {
        public int BookingId { get; set; }
        public List<string> SeatNumbers { get; set; }
        public string Reason { get; set; }
    }
}
