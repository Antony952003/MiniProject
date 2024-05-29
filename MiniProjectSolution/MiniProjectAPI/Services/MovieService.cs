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
        /// <summary>
        /// Adds a new movie to the system.
        /// </summary>
        /// <param name="movieInputDTO">The movie input DTO containing information about the movie to be added.</param>
        /// <returns>A movie return DTO containing information about the added movie.</returns>
        /// <exception cref="EntityAlreadyExists">Thrown when a movie with the same title already exists.</exception>
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
        /// <summary>
        /// Maps a movie entity to a movie return DTO.
        /// </summary>
        /// <param name="movie">The movie entity to be mapped.</param>
        /// <returns>A movie return DTO containing information mapped from the provided movie entity.</returns>

        private MovieReturnDTO? MapMovieToReturnDTO(Movie movie)
        {
            MovieReturnDTO returnDTO = new MovieReturnDTO()
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Cast = movie.Cast,
                Director = movie.Director,
                DurationInHours = movie.DurationInHours,
                AverageRating = 0
            };
            return returnDTO;
        }
        /// <summary>
        /// Maps a movie input DTO to a movie entity.
        /// </summary>
        /// <param name="movieInputDTO">The movie input DTO to be mapped.</param>
        /// <returns>A movie entity mapped from the provided movie input DTO.</returns>

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
        public async Task<List<MovieReturnDTO>> SortMoviesByReviews()
        {
            var moviesWithReviews = await _movieRepo.Get();
            var sortedMovies = moviesWithReviews
                .Select(movie => new
                {
                    Movie = movie,
                    AverageRating = movie.Reviews.Any() ? movie.Reviews.Average(r => r.Rating) : 0
                })
                .OrderByDescending(m => m.AverageRating)
                .Select(m => new MovieReturnDTO
                {
                    MovieId = m.Movie.MovieId,
                    Title = m.Movie.Title,
                    Description = m.Movie.Description,
                    Genre = m.Movie.Genre,
                    Cast = m.Movie.Cast,
                    Director = m.Movie.Director,
                    DurationInHours = m.Movie.DurationInHours,
                    AverageRating = m.AverageRating
                })
                .ToList();

            return sortedMovies;
        }
    }
}
