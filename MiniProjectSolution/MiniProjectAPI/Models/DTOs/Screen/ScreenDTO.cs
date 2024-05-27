namespace MovieBookingAPI.Models.DTOs.Screen
{
    public class ScreenDTO
    {
        public int ScreenId { get; set; }
        public int TheaterId { get; set; }
        public string Name { get; set; }
        public int SeatingCapacity { get; set; }
    }
}
