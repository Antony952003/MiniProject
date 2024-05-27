using MiniProjectAPI.Models;
using MovieBookingAPI.Models.DTOs.Snack;

namespace MovieBookingAPI.Interfaces
{
    public interface ISnackService
    {
        public Task<Snack> AddSnack(SnackInputDTO snackInputDTO);
        public Task<Snack> GetSnackIdByName(string name);
    }
}
