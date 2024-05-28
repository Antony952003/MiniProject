namespace MovieBookingAPI.Models.DTOs.Payment
{
    public class PaymentInputDTO
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
