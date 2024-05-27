namespace MovieBookingAPI.Models.DTOs.Movie
{
    public class MovieInputDTO
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public double DurationInHours { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
    }
}
