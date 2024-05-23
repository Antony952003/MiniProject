namespace MiniProjectAPI.Models
{
    public class Showtime
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public DateTime StartTime { get; set; }
        public Movie Movie { get; set; }
        public Screen Screen { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}