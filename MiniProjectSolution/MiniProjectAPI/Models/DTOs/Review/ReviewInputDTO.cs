namespace MovieBookingAPI.Models.DTOs.Review
{
    public class ReviewInputDTO
    {
        public int MovieId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
