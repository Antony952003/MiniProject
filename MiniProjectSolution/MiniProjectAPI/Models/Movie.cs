namespace MiniProjectAPI.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public double DurationInHours { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}