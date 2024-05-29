using MiniProjectAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingAPI.Models.DTOs.Booking
{
    public class BookingInputDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ShowtimeId { get; set; }
        [Required]
        public int MovieId { get; set; }
        public int? PointsToRedeem { get; set; }
        public Dictionary<string, int> BookingSnacks { get; set; }
        [Required]
        public List<string> BookingSeats { get; set; }

    }
}
