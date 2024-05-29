using System.ComponentModel.DataAnnotations;

namespace MovieBookingAPI.Models.DTOs.Register
{
    public class RegisterInputDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MinLength(10, ErrorMessage = "Phone Number has to 10 digits long")]
        public string Phone { get; set; }
        public string Image { get; set; }
        public string? Role { get; set; }
        [MinLength(6, ErrorMessage = "Password has to be minimum 6 chars long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
        [MinLength(6, ErrorMessage = "Password has to be minimum 6 chars long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string ConfirmPassword { get; set; }

    }
}
