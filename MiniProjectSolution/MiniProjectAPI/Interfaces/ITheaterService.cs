using MovieBookingAPI.Models.DTOs.Theater;

namespace MovieBookingAPI.Interfaces
{
    public interface ITheaterService
    {
        public Task<TheaterDTO> AddTheater(TheaterDTO theaterDTO);
    }
}
