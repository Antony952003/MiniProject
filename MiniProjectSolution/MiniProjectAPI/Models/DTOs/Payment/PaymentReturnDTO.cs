namespace MovieBookingAPI.Models.DTOs.Payment
{
    public class PaymentReturnDTO
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string PaymentStatus { get; set; }
        public Dictionary<string, decimal> TicketPrice { get; set; }
        public Dictionary<string, decimal> ConvenienceFees { get; set; }
        public Dictionary<string, int> FoodBeverages { get; set; }
        public string PaymentMethod { get; set; }
        public decimal OrderTotal { get; set; }

    }
}
