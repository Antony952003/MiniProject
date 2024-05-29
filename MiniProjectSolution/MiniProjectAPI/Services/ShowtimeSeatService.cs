using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.ShowtimeSeat;

namespace MovieBookingAPI.Services
{
    public class ShowtimeSeatService : IShowtimeSeatService
    {
        private readonly IRepository<int, ShowtimeSeat> _showtimeseatRepo;
        private readonly IRepository<int, Screen> _screenRepo;
        private readonly IRepository<int, Seat> _seatRepo;

        public ShowtimeSeatService(IRepository<int, ShowtimeSeat> showtimeseatRepo, IRepository<int, Screen> screenRepo,
            IRepository<int, Seat> seatRepo) {
            _showtimeseatRepo = showtimeseatRepo;
            _screenRepo = screenRepo;
            _seatRepo = seatRepo;
        }
        /// <summary>
        /// Generates showtime seats for a given showtime and screen.
        /// </summary>
        /// <param name="showtimeseatGenerateDTO">The DTO containing information required to generate showtime seats.</param>
        /// <returns>An enumerable collection of showtime seat return DTOs containing information about the generated showtime seats.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the specified screen is not found.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during seat generation.</exception>

        public async Task<IEnumerable<ShowtimeSeatReturnDTO>> GenerateShowtimeSeats(ShowtimeSeatGenerateDTO showtimeseatGenerateDTO)
        {
            try
            {
                var screen = await _screenRepo.Get(showtimeseatGenerateDTO.ScreenId);
                if (screen == null)
                {
                    throw new EntityNotFoundException("Screen");
                }
                var seats = await _seatRepo.Get();
                List<ShowtimeSeatReturnDTO> result = new List<ShowtimeSeatReturnDTO>();
                seats = seats.ToList().Where(x => x.ScreenId == showtimeseatGenerateDTO.ScreenId).ToList();
                foreach (var seat in seats)
                {
                    ShowtimeSeat showtimeSeat = new ShowtimeSeat
                    {
                        SeatId = seat.SeatId,
                        ShowtimeId = showtimeseatGenerateDTO.ShowtimeId,
                        Status = "Available",
                    };
                    await _showtimeseatRepo.Add(showtimeSeat);
                    result.Add(MapShowtimeSeatWithReturnDTO(showtimeSeat));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Retrieves available showtime seats for a given showtime ID.
        /// </summary>
        /// <param name="showtimeId">The ID of the showtime for which available seats are to be retrieved.</param>
        /// <returns>A list of showtime seat return DTOs containing information about the available showtime seats.</returns>
        /// <exception cref="Exception">Thrown when no seats are available for the specified showtime.</exception>

        public async Task<List<ShowtimeSeatReturnDTO>> GetAvailableShowtimeSeats(int showtimeId)
        {
            var showtimeseats = await _showtimeseatRepo.Get();
            showtimeseats = showtimeseats.Where(x => x.Status == "Available" && x.ShowtimeId == showtimeId);
            if(showtimeseats == null)
            {
                throw new Exception("No Seats Available for the Showtime");
            }
            List<ShowtimeSeatReturnDTO> result = new List<ShowtimeSeatReturnDTO>();
            foreach(var showtimeseat in showtimeseats)
            {
                result.Add(MapShowtimeSeatWithReturnDTO(showtimeseat));
            }
            return result;
        }

        public async Task<List<List<ShowtimeSeatReturnDTO>>> GetConsecutiveSeatsInRange(ShowtimeSeatConsecutiveRangeDTO showtimeSeatConsecutiveRangeDTO)
        {
            var numberOfSeats = showtimeSeatConsecutiveRangeDTO.NumberOfSeats;
            var seatsInRange = await _seatRepo.Get();
            seatsInRange = seatsInRange
            .Where(seat => seat.Row[0] >= showtimeSeatConsecutiveRangeDTO.StartRow[0] && seat.Row[0] <= showtimeSeatConsecutiveRangeDTO.EndRow[0])
            .OrderBy(seat => seat.Row)
            .ThenBy(seat => seat.Column)
            .ToList();

            var showtimeSeats = await _showtimeseatRepo.Get();
            showtimeSeats = showtimeSeats
                .Where(sts => sts.ShowtimeId == showtimeSeatConsecutiveRangeDTO.ShowtimeId && seatsInRange.Select(s => s.SeatId).Contains(sts.SeatId))
                .ToList();

            var consecutiveSeats = new List<List<ShowtimeSeatReturnDTO>>();

            foreach (var row in seatsInRange.Select(s => s.Row))
            {
                var seatsInRow = showtimeSeats.Where(sts => sts.Seat.Row == row && sts.Status == "Available").OrderBy(sts => sts.Seat.Row)
                    .ThenBy(sts => sts.Seat.Column)
                    .ToList();
                for (int i = 0; i <= seatsInRow.Count() - numberOfSeats; i++)
                {
                    bool isConsecutive = true;
                    for (int j = 0; j < numberOfSeats - 1; j++)
                    {
                        if (seatsInRow[i + j].Seat.Column + 1 != seatsInRow[i + j + 1].Seat.Column)
                        {
                            isConsecutive = false;
                            break;
                        }
                    }
                    if (isConsecutive)
                    {
                        consecutiveSeats.Add(seatsInRow.Skip(i).Take(numberOfSeats).Select(sts => new ShowtimeSeatReturnDTO
                        {
                            ShowtimeSeatId = sts.ShowtimeSeatId,
                            SeatId = sts.SeatId,
                            SeatNumber = sts.Seat.SeatNumber,
                            Status = sts.Status,
                            Price = sts.Seat.Price
                        }).ToList());
                    }
                }
            }

            return consecutiveSeats;
        }

        /// <summary>
        /// Retrieves the showtime seat IDs for the given seat numbers and showtime ID.
        /// </summary>
        /// <param name="seatnumbers">The list of seat numbers to retrieve showtime seat IDs for.</param>
        /// <param name="showtimeId">The ID of the showtime for which showtime seat IDs are to be retrieved.</param>
        /// <returns>A list of showtime seat IDs corresponding to the provided seat numbers and showtime ID.</returns>
        /// <exception cref="NoEntitiesFoundException">Thrown when no showtime seats are found.</exception>

        public async Task<List<int>> GetShowtimeIdsForSeatNumbers(List<string> seatnumbers, int showtimeId)
        {

            var AllShowtimeseats = await _showtimeseatRepo.Get();
            if (!AllShowtimeseats.Any()) throw new NoEntitiesFoundException("ShowtimeSeat");
            var result = AllShowtimeseats.ToList()
                .Where(ss => ss.ShowtimeId == showtimeId && seatnumbers
                .Contains(ss.Seat.SeatNumber))
                .Select(ss => ss.ShowtimeSeatId).ToList();
            return result;
            
        }

        public async  Task<List<ShowtimeSeatReturnDTO>> GetShowtimeSeatsInRange(ShowtimeSeatRangeDTO showtimeSeatRangeDTO)
        {
            var seatsInRange = await _seatRepo.Get();
            seatsInRange = seatsInRange
            .Where(seat => seat.Row[0] >= showtimeSeatRangeDTO.StartRow[0] && seat.Row[0] <= showtimeSeatRangeDTO.EndRow[0])
            .OrderBy(seat => seat.Row)
            .ThenBy(seat => seat.SeatNumber)
            .Take((showtimeSeatRangeDTO.EndRow[0] - showtimeSeatRangeDTO.StartRow[0]) * 15) // Assuming each row has 10 seats, adjust as needed
            .ToList();

            var showtimeSeats = await _showtimeseatRepo.Get();
            showtimeSeats = showtimeSeats
                .Where(sts => sts.ShowtimeId == showtimeSeatRangeDTO.ShowtimeId && sts.Status == "Available" && seatsInRange.Select(s => s.SeatId).Contains(sts.SeatId))
                .ToList();

            var result = showtimeSeats.Select(sts => new ShowtimeSeatReturnDTO
            {
                ShowtimeSeatId = sts.ShowtimeSeatId,
                SeatId = sts.SeatId,
                SeatNumber = sts.Seat.SeatNumber,
                Status = sts.Status,
                Price = sts.Seat.Price,
                
            }).ToList();

            return result;
        }
        /// <summary>
        /// Updates the status of showtime seats for a given showtime ID and list of showtime seat IDs.
        /// </summary>
        /// <param name="showtimeId">The ID of the showtime for which showtime seat statuses are to be updated.</param>
        /// <param name="showtimeseatIds">The list of showtime seat IDs whose statuses are to be updated.</param>
        /// <param name="status">The new status to be assigned to the showtime seats.</param>
        /// <returns>A list of updated showtime seats.</returns>
        /// <exception cref="NoEntitiesFoundException">Thrown when no showtime seats are found.</exception>
        /// <exception cref="TicketAlreadyBookedException">Thrown when attempting to update a showtime seat that is already booked.</exception>

        public async Task<List<ShowtimeSeat>> UpdateShowtimeSeatsStatus(int showtimeId, List<int> showtimeseatIds, string status)
        {
            var AllShowtimeseats = await _showtimeseatRepo.Get();
            if (!AllShowtimeseats.Any()) throw new NoEntitiesFoundException("ShowtimeSeat");
            var showtimeSeats = AllShowtimeseats.ToList()
                .Where(ss => ss.ShowtimeId == showtimeId && showtimeseatIds.Contains(ss.ShowtimeSeatId))
                .ToList();
            foreach (var showtimeSeat in showtimeSeats)
            {
                if (showtimeSeat.Status == status) throw new TicketAlreadyBookedException(showtimeSeat.Seat.SeatNumber);
                showtimeSeat.Status = status;
                await _showtimeseatRepo.Update(showtimeSeat);
            }
            return showtimeSeats;

        }
        /// <summary>
        /// Maps a showtime seat entity to a showtime seat return DTO.
        /// </summary>
        /// <param name="showtimeSeat">The showtime seat entity to be mapped.</param>
        /// <returns>A showtime seat return DTO containing information mapped from the provided showtime seat entity.</returns>

        private ShowtimeSeatReturnDTO MapShowtimeSeatWithReturnDTO(ShowtimeSeat showtimeSeat)
        {
            ShowtimeSeatReturnDTO showtimeseatreturn = new ShowtimeSeatReturnDTO()
            {
                ShowtimeSeatId = showtimeSeat.ShowtimeSeatId,
                SeatId = showtimeSeat.SeatId,
                Price = showtimeSeat.Seat.Price,
                SeatNumber = showtimeSeat.Seat.SeatNumber,
                Status = showtimeSeat.Status,
            };
            return showtimeseatreturn;
        }
    }

        
    }
