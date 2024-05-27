using MovieBookingAPI.Models.DTOs.Seat;

namespace MovieBookingAPI.Interfaces
{
    public interface ISeatService
    {
        public Task<IEnumerable<SeatOutputDTO>> GenerateSeatsForScreen(SeatGenerationInputDTO seatgenerateDTO);
    }
}
