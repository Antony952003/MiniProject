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
