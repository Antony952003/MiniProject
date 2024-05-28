using MiniProjectAPI.Models;

namespace MovieBookingAPI.Models.DTOs.Booking
{
    public class BookingInputDTO
    {
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public int PointsToRedeem { get; set; }
        public Dictionary<string, int> BookingSnacks { get; set; }
        public List<string> BookingSeats { get; set; }

    }
}
