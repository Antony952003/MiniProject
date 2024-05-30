using System.ComponentModel.DataAnnotations;

namespace MovieBookingAPI.Models.DTOs.Review
{
    public class ReviewInputDTO
    {
        [Required]
        public int MovieId { get; set; }
        [Required(ErrorMessage = "Comment cannot be empty")]
        public string Comment { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating Should be within range of 1 - 5")]
        public int Rating { get; set; }
    }
}
