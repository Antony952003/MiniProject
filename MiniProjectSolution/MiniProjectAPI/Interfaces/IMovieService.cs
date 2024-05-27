using MovieBookingAPI.Models.DTOs.Movie;

namespace MovieBookingAPI.Interfaces
{
    public interface IMovieService
    {
        public Task<MovieReturnDTO> AddMovie(MovieInputDTO movieInputDTO);
    }
}
