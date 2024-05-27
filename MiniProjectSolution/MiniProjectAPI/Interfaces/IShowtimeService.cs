using MovieBookingAPI.Models.DTOs.Showtime;

namespace MovieBookingAPI.Interfaces
{
    public interface IShowtimeService
    {
        public Task<ShowtimeReturnDTO> AddShowtime(ShowtimeInputDTO showtimeInputDTO);
    }
}
