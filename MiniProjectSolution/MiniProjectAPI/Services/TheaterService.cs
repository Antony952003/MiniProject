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
    }
}
