using MovieBookingAPI.Models.DTOs.Theater;

namespace MovieBookingAPI.Interfaces
{
    public interface ITheaterService
    {
        public Task<TheaterDTO> AddTheater(TheaterDTO theaterDTO);
        public Task<TheaterDTO> GetTheaterById(int theaterId);
        public Task<TheaterDTO> GetTheaterByName(string theaterName);
        public Task<TheaterDTO> UpdateTheaterLocation(int theaterid, string location);
    }
}
