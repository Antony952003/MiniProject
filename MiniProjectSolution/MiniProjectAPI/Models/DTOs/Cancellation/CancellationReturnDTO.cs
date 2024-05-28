namespace MovieBookingAPI.Models.DTOs.Cancellation
{
    public class CancellationReturnDTO
    {
        public int CancellationId { get; set; }
        public int BookingId { get; set; }
        public DateTime CancellationDate { get; set; }
        public decimal RefundAmount { get; set; }
        public List<string> CancelledSeatNumbers { get; set; }
    }
}
