using MovieBookingAPI.Models;

namespace MiniProjectAPI.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int ScreenId { get; set; }
        public string SeatNumber { get; set; }
        public string Row { get; set; }
        public int Column { get; set; }
        public decimal Price { get; set; }
        public Screen Screen { get; set; }
        public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; }
    }
}