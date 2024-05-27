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
