using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Showtime;

namespace MovieBookingAPI.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IRepository<int, Showtime> _showtimeRepo;
        private readonly IRepository<int, Movie> _movieRepo;
        private readonly IRepository<int, Screen> _screenRepo;

        public ShowtimeService(IRepository<int, Showtime> showtimeRepo, IRepository<int, Movie> movieRepo,
            IRepository<int, Screen> screenRepo) { 
            _showtimeRepo = showtimeRepo;
            _movieRepo = movieRepo;
            _screenRepo = screenRepo;
        }
        /// <summary>
        /// Adds a new showtime to the system.
        /// </summary>
        /// <param name="showtimeInputDTO">The showtime input DTO containing information about the showtime to be added.</param>
        /// <returns>A showtime return DTO containing information about the added showtime.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs while adding the showtime.</exception>

        public async Task<ShowtimeReturnDTO> AddShowtime(ShowtimeInputDTO showtimeInputDTO)
        {
            Showtime showtime = null;
            ShowtimeReturnDTO result  = null;
            try
            {
                showtime = MapInputToShowtime(showtimeInputDTO);
                showtime = await _showtimeRepo.Add(showtime);
                var movie = await _movieRepo.Get(showtime.MovieId);
                var screen = await _screenRepo.Get(showtime.ScreenId);
                var moviename = movie.Title;
                var screenname = screen.Name;
                result = MapShowtimeToReturn(showtime, moviename, screenname);
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Maps a showtime entity to a showtime return DTO.
        /// </summary>
        /// <param name="showtime">The showtime entity to be mapped.</param>
        /// <param name="moviename">The name of the movie associated with the showtime.</param>
        /// <param name="screenname">The name of the screen associated with the showtime.</param>
        /// <returns>A showtime return DTO containing information mapped from the provided showtime entity.</returns>

        private ShowtimeReturnDTO? MapShowtimeToReturn(Showtime showtime, string moviename, string screenname)
        {
            ShowtimeReturnDTO showtimeReturnDTO = new ShowtimeReturnDTO()
            {
                MovieName = moviename,
                ScreenName = screenname,
                StartTime = showtime.StartTime
            };
            return showtimeReturnDTO;
        }
        /// <summary>
        /// Maps a showtime input DTO to a showtime entity.
        /// </summary>
        /// <param name="showtimeInputDTO">The showtime input DTO to be mapped.</param>
        /// <returns>A showtime entity mapped from the provided showtime input DTO.</returns>

        private Showtime? MapInputToShowtime(ShowtimeInputDTO showtimeInputDTO)
        {
            Showtime showtime = new Showtime()
            {
                ScreenId = showtimeInputDTO.ScreenId,
                MovieId = showtimeInputDTO.MovieId,
                StartTime = DateTime.Parse(showtimeInputDTO.StartTime),
            };
            return showtime;
        }
    }
}
