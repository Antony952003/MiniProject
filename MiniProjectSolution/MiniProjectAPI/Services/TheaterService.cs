using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Theater;

namespace MovieBookingAPI.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly IRepository<int, Theater> _theaterRepo;
        public TheaterService(IRepository<int, Theater> theaterRepo)
        {
            _theaterRepo = theaterRepo;
        }
        /// <summary>
        /// Adds a new theater to the system.
        /// </summary>
        /// <param name="theaterDTO">The theater DTO containing information about the theater to be added.</param>
        /// <returns>A theater DTO containing information about the added theater.</returns>
        /// <exception cref="EntityAlreadyExists">Thrown when the theater with the provided ID already exists.</exception>

        public async Task<TheaterDTO> AddTheater(TheaterDTO theaterDTO)
        {
            Theater theater = null;
            TheaterDTO result = null;
            try
            {
                var isTheaterExists = await _theaterRepo.Get(theaterDTO.TheaterId);
                if (isTheaterExists != null)
                {
                    throw new EntityAlreadyExists("Theater");
                }
                theater = MapInputDTOWithTheater(theaterDTO);
                theater = await _theaterRepo.Add(theater);
                result = MapTheaterWithReturnDTO(theater);
                return result;
            }
            catch (Exception ex)
            {
                throw new EntityAlreadyExists("Theater");
            }
        }

        private TheaterDTO? MapTheaterWithReturnDTO(Theater theater)
        {
            TheaterDTO result = new TheaterDTO()
            {
                TheaterId = theater.TheaterId,
                Name = theater.Name,
                Location = theater.Location,
            };
            return result;
        }

        private Theater? MapInputDTOWithTheater(TheaterDTO theaterDTO)
        {
            Theater theater = new Theater()
            {
                Name = theaterDTO.Name,
                Location = theaterDTO.Location,
            };
            return theater;
        }
        /// <summary>
        /// Retrieves a theater by its ID.
        /// </summary>
        /// <param name="theaterId">The ID of the theater to retrieve.</param>
        /// <returns>A theater DTO containing information about the retrieved theater.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the theater with the provided ID is not found.</exception>

        public async Task<TheaterDTO> GetTheaterById(int theaterId)
        {
            var theater = await _theaterRepo.Get(theaterId);
            if(theater == null)
            {
                throw new EntityNotFoundException("Theater");
            }
            var result = MapTheaterWithReturnDTO(theater);
            return result;
        }
        /// <summary>
        /// Retrieves a theater by its name.
        /// </summary>
        /// <param name="theaterName">The name of the theater to retrieve.</param>
        /// <returns>A theater DTO containing information about the retrieved theater.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the theater with the provided name is not found.</exception>

        public async Task<TheaterDTO> GetTheaterByName(string theaterName)
        {
            var theaters = await _theaterRepo.Get();
            var theater = theaters.ToList().FirstOrDefault(x => x.Name == theaterName);
            if(theater != null)
            {
                return MapTheaterWithReturnDTO(theater);
            }
            throw new EntityNotFoundException("Theater");
        }
        /// <summary>
        /// Updates the location of a theater.
        /// </summary>
        /// <param name="theaterid">The ID of the theater to update.</param>
        /// <param name="location">The new location to be assigned to the theater.</param>
        /// <returns>A theater DTO containing information about the updated theater.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the theater with the provided ID is not found.</exception>

        public async Task<TheaterDTO> UpdateTheaterLocation(int theaterid, string location)
        {
            var theater = await _theaterRepo.Get(theaterid);
            if (theater == null) throw new EntityNotFoundException("Theater");
            theater.Location = location;
            theater = await _theaterRepo.Update(theater);
            return MapTheaterWithReturnDTO(theater);
        }
    }
}
