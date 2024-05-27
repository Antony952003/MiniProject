using MovieBookingAPI.Models.DTOs.Screen;

namespace MovieBookingAPI.Interfaces
{
    public interface IScreenService
    {
        public Task<ScreenOutputDTO> AddScreenToTheater(ScreenDTO screenDTO);
    }
}
