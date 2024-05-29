namespace MovieBookingAPI.Models.DTOs.Movie
{
    public class MovieReturnDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Cast { get; set; }
        public string Director { get; set; }
        public double DurationInHours { get; set; }
        public double AverageRating { get; set; }
    }
}
