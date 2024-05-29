using MiniProjectAPI.Models;
using MovieBookingAPI.Models.DTOs.Snack;

namespace MovieBookingAPI.Interfaces
{
    public interface ISnackService
    {
        public Task<SnackReturnDTO> AddSnack(SnackInputDTO snackInputDTO);
        public Task<Snack> GetSnackIdByName(string name);
        public Task<SnackReturnDTO> UpdateSnackPrice(string name, decimal newPrice);

    }
}
