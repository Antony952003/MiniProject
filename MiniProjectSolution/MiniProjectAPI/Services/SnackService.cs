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
        /// <summary>
        /// Adds a new snack to the system.
        /// </summary>
        /// <param name="snackInputDTO">The snack input DTO containing information about the snack to be added.</param>
        /// <returns>A snack return DTO containing information about the added snack.</returns>

        public async Task<SnackReturnDTO> AddSnack(SnackInputDTO snackInputDTO)
        {
            Snack snack = null;
            snack = MapSnackInput(snackInputDTO);
            snack = await _snackRepo.Add(snack);

            return new SnackReturnDTO()
            {
                Name = snack.Name,
                Price = snack.Price,
                SnackId = snack.SnackId,
            };
        }
        /// <summary>
        /// Retrieves the snack ID by its name.
        /// </summary>
        /// <param name="name">The name of the snack to retrieve the ID for.</param>
        /// <returns>The snack entity corresponding to the provided name.</returns>
        /// <exception cref="NoEntitiesFoundException">Thrown when no snacks are found.</exception>

        public async Task<Snack> GetSnackIdByName(string name)
        {
            var snacks = await _snackRepo.Get();
            if (!snacks.Any()) throw new NoEntitiesFoundException("Snack");
            var snack = snacks.FirstOrDefault(x => x.Name == name);
            return snack;
        }
        /// <summary>
        /// Updates the price of a snack by its name.
        /// </summary>
        /// <param name="name">The name of the snack to update the price for.</param>
        /// <param name="newPrice">The new price to be assigned to the snack.</param>
        /// <returns>A snack return DTO containing information about the updated snack.</returns>

        public async Task<SnackReturnDTO> UpdateSnackPrice(string name, decimal newPrice)
        {
            var snack = await GetSnackIdByName(name);
            snack.Price = newPrice;
            snack = await _snackRepo.Update(snack);
            return new SnackReturnDTO()
            {
                Name = snack.Name,
                Price = snack.Price,
                SnackId = snack.SnackId,
            };

        }
        /// <summary>
        /// Maps a snack input DTO to a snack entity.
        /// </summary>
        /// <param name="snackInputDTO">The snack input DTO to be mapped.</param>
        /// <returns>A snack entity mapped from the provided snack input DTO.</returns>

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
