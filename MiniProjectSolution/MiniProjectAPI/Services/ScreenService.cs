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
        /// <summary>
        /// Adds a new screen to a theater.
        /// </summary>
        /// <param name="screenDTO">The screen DTO containing information about the screen to be added.</param>
        /// <returns>A screen output DTO containing information about the added screen.</returns>
        /// <exception cref="EntityAlreadyExists">Thrown when a screen with the same ID already exists or an unexpected error occurs.</exception>

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
        /// <summary>
        /// Retrieves screens by theater name.
        /// </summary>
        /// <param name="theaterName">The name of the theater for which screens are to be retrieved.</param>
        /// <returns>A list of screen output DTOs containing information about screens in the specified theater.</returns>
        /// <exception cref="Exception">Thrown when there are no screens available for the specified theater name.</exception>

        public async Task<List<ScreenOutputDTO>> GetScreensByTheaterName(string theaterName)
        {
            var screens = await _repository.Get();
            screens = screens.ToList().FindAll(x => x.Theater.Name == theaterName).ToList();
            if(screens == null)
            {
                throw new Exception("There is no screens for theater name " + theaterName);
            }
            List<ScreenOutputDTO> result = new List<ScreenOutputDTO>();  
            foreach(var screen in screens)
            {
                result.Add(MapScreenWithOutputDTO(screen, theaterName));
            }
            return result;
             
        }
        /// <summary>
        /// Retrieves a screen by its name.
        /// </summary>
        /// <param name="screenName">The name of the screen to be retrieved.</param>
        /// <returns>A screen output DTO containing information about the retrieved screen.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the specified screen is not found.</exception>

        public async Task<ScreenOutputDTO> GetScreenByScreenName(string screenName)
        {
            var screens = await _repository.Get();
            var screen = screens.ToList().FirstOrDefault(x => x.Name == screenName);
            if(screen  != null) 
                return MapScreenWithOutputDTO(screen, screen.Theater.Name);
            throw new EntityNotFoundException("Screen");
        }
        /// <summary>
        /// Retrieves a screen by its ID.
        /// </summary>
        /// <param name="screenId">The ID of the screen to be retrieved.</param>
        /// <returns>A screen output DTO containing information about the retrieved screen.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the specified screen is not found.</exception>

        public async Task<ScreenOutputDTO> GetScreenByScreenId(int screenId)
        {
            var screen = await _repository.Get(screenId);
            if(screen != null) return MapScreenWithOutputDTO(screen, screen.Theater.Name);
            throw new EntityNotFoundException("Screen");
        }
    }
}
