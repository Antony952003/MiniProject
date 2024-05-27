namespace MovieBookingAPI.Models.DTOs.Register
{
    public class RegisterInputDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
