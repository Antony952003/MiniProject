namespace MovieBookingAPI.Models.DTOs.Review
{
    public class ReviewReturnDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string MovieName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
