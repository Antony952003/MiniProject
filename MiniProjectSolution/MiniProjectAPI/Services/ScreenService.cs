using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Screen;

namespace MovieBookingAPI.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IRepository<int, Screen> _repository;
        private readonly IRepository<int, Theater> _theaterRepo;

        public ScreenService(IRepository<int, Screen> repository,
            IRepository<int, Theater> theaterRepo) { 
            _repository = repository;
            _theaterRepo = theaterRepo;
        }
        public async Task<ScreenOutputDTO> AddScreenToTheater(ScreenDTO screenDTO)
        {
            ScreenOutputDTO screenOutputDTO = null;
            Screen screen = null;
            try
            {
                screen = await MapScreenDTOWithScreen(screenDTO);
                if(screen == null) {
                    throw new EntityAlreadyExists("Screen Already Exists");
                }
                screen = await _repository.Add(screen);
                var theater = await _theaterRepo.Get(screen.TheaterId);
                if(theater == null) {
                    throw new EntityNotFoundException("Theater");
                }
                screenOutputDTO = MapScreenWithOutputDTO(screen, theater.Name);
                return screenOutputDTO;
                
            }
            catch (Exception ex)
            {
                throw new EntityAlreadyExists("Screen");
            }
        }

        private ScreenOutputDTO? MapScreenWithOutputDTO(Screen screen, string name)
        {
            ScreenOutputDTO result = new ScreenOutputDTO()
            {
                ScreenId = screen.ScreenId,
                ScreenName = screen.Name,
                SeatingCapacity = screen.SeatingCapacity,
                TheaterName = name
            };
            return result;
        }

        private async Task<Screen?> MapScreenDTOWithScreen(ScreenDTO screenDTO)
        {
            var isscreen = await _repository.Get(screenDTO.ScreenId);
            if (isscreen != null) return null;
            Screen screen = new Screen()
            {
                Name = screenDTO.Name,
                SeatingCapacity = screenDTO.SeatingCapacity,
                TheaterId = screenDTO.TheaterId,
            };
            return screen;
        }
    }
}
