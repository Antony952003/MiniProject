namespace MovieBookingAPI.Models.DTOs.Showtime
{
    public class ShowtimeInputDTO
    {
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public string StartTime { get; set; }
    }
}
