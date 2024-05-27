using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Movie;

namespace MovieBookingAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly IRepository<int, Movie> _movieRepo;

        public MovieService(IRepository<int, Movie> movieRepo) { 
            _movieRepo = movieRepo;
        }
        public async Task<MovieReturnDTO> AddMovie(MovieInputDTO movieInputDTO)
        {
            var movies = await _movieRepo.Get();
            if(movies != null) {
                var ispresent = movies.ToList().FirstOrDefault(x => x.Title == movieInputDTO.Title);
                if (ispresent != null)
                {
                    throw new EntityAlreadyExists("Movie");
                }
            }
            Movie movie = null;
            MovieReturnDTO result = null;
            try
            {
                movie = MapInputDTOToMovie(movieInputDTO);
                movie = await _movieRepo.Add(movie);
                result = MapMovieToReturnDTO(movie);
                return result;
            }
            catch (Exception ex)
            {
                throw new EntityAlreadyExists("Movie");
            }
        }

        private MovieReturnDTO? MapMovieToReturnDTO(Movie movie)
        {
            MovieReturnDTO returnDTO = new MovieReturnDTO()
            {
                Title = movie.Title,
                Message = "Movie Added Succesfully!!"
            };
            return returnDTO;
        }

        private Movie? MapInputDTOToMovie(MovieInputDTO movieInputDTO)
        {
            Movie movie = new Movie()
            {
                Title = movieInputDTO.Title,
                Description = movieInputDTO.Description,
                Genre = movieInputDTO.Genre,
                Cast = movieInputDTO.Cast,
                Director = movieInputDTO.Director,
                DurationInHours = movieInputDTO.DurationInHours,
            };
            return movie;
        }
    }
}
