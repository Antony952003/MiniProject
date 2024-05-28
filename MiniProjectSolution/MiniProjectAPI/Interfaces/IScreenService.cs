using MiniProjectAPI.Models;
using MovieBookingAPI.Models.DTOs.Screen;

namespace MovieBookingAPI.Interfaces
{
    public interface IScreenService
    {
        public Task<ScreenOutputDTO> AddScreenToTheater(ScreenDTO screenDTO);
        public Task<List<ScreenOutputDTO>> GetScreensByTheaterName(string theaterName);
        public Task<ScreenOutputDTO> GetScreenByScreenName(string screenName);
        public Task<ScreenOutputDTO> GetScreenByScreenId(int screenId);

    }
}
