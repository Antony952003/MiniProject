using MovieBookingAPI.Models.DTOs.Cancellation;

namespace MovieBookingAPI.Interfaces
{
    public interface ICancellationService
    {
        public Task<CancellationReturnDTO> ProcessCancellation(CancellationInputDTO cancellationInputDTO);
    }
}
