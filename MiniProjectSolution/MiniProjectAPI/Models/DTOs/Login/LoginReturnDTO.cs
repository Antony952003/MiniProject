namespace MovieBookingAPI.Models.DTOs.Login
{
    public class LoginReturnDTO
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
