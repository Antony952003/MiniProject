using MiniProjectAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingAPI.Models
{
    public class UserPoint
    {
        [Key]
        public int UserPointsId { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public DateTime LastUpdated { get; set; }
        public User User { get; set; }

    }
}
