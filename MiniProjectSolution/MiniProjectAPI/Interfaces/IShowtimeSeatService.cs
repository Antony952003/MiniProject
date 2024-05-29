using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.ShowtimeSeat;

namespace MovieBookingAPI.Interfaces
{
    public interface IShowtimeSeatService
    {
        public Task<IEnumerable<ShowtimeSeatReturnDTO>> GenerateShowtimeSeats(ShowtimeSeatGenerateDTO showtimeseatGenerateDTO);
        public Task<List<int>> GetShowtimeIdsForSeatNumbers(List<string> seatnumbers, int showtimeId);
        public Task<List<ShowtimeSeat>> UpdateShowtimeSeatsStatus(int showtimeId, List<int> seatIds, string status);
        public Task<List<ShowtimeSeatReturnDTO>> GetAvailableShowtimeSeats(int showtimeId);
        Task<List<ShowtimeSeatReturnDTO>> GetShowtimeSeatsInRange(ShowtimeSeatRangeDTO showtimeSeatRangeDTO);
        Task<List<List<ShowtimeSeatReturnDTO>>> GetConsecutiveSeatsInRange(ShowtimeSeatConsecutiveRangeDTO showtimeSeatConsecutiveRangeDTO);
    }
}
