namespace MovieBookingAPI.Models.DTOs.ShowtimeSeat
{
    public class ShowtimeSeatReturnDTO
    {
        public int ShowtimeSeatId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}
