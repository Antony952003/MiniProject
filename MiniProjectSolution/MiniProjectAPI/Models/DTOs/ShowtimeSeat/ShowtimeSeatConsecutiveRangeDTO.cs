namespace MovieBookingAPI.Models.DTOs.ShowtimeSeat
{
    public class ShowtimeSeatConsecutiveRangeDTO
    {
        public int ShowtimeId { get; set; }
        public string StartRow { get; set; }
        public string EndRow { get; set; }
        public int NumberOfSeats { get; set; }
    }
}
