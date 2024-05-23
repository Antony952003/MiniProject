namespace MiniProjectAPI.Models
{
    public class Screen
    {
        public int ScreenId { get; set; }
        public int TheaterId { get; set; }
        public string Name { get; set; }
        public int SeatingCapacity { get; set; }

        public Theater Theater { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}