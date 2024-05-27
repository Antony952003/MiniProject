using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Snack;

namespace MovieBookingAPI.Services
{
    public class SnackService : ISnackService
    {
        private readonly IRepository<int, Snack> _snackRepo;

        public SnackService(IRepository<int, Snack> snackRepo)
        {
            _snackRepo = snackRepo;
        }
        public async Task<Snack> AddSnack(SnackInputDTO snackInputDTO)
        {
            Snack snack = null;
            snack = MapSnackInput(snackInputDTO);
            snack = await _snackRepo.Add(snack);
            return snack;
        }
        public async Task<Snack> GetSnackIdByName(string name)
        {
            var snacks = await _snackRepo.Get();
            if (!snacks.Any()) throw new NoEntitiesFoundException("Snack");
            var snack = snacks.FirstOrDefault(x => x.Name == name);
            return snack;
        }

        private Snack? MapSnackInput(SnackInputDTO snackInputDTO)
        {
            Snack snack = new Snack()
            {
                Name = snackInputDTO.Name,
                Price = snackInputDTO.Price,
            };
            return snack;
        }
    }
}
