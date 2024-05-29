using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Seat;

namespace MovieBookingAPI.Services
{
    public class SeatService : ISeatService
    {
        private readonly IRepository<int, Seat> _seatRepo;
        private readonly IRepository<int, Screen> _screenRepo;

        public SeatService(IRepository<int, Seat> seatRepo, IRepository<int, Screen> screenRepo) {
            _seatRepo = seatRepo;
            _screenRepo = screenRepo;
        }
        /// <summary>
        /// Generates seats for a screen based on the provided seat generation input DTO.
        /// </summary>
        /// <param name="seatgenerateDTO">The seat generation input DTO containing information about the seats to be generated.</param>
        /// <returns>An enumerable collection of seat output DTOs containing information about the generated seats.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the specified screen is not found.</exception>

        public async Task<IEnumerable<SeatOutputDTO>> GenerateSeatsForScreen(SeatGenerationInputDTO seatgenerateDTO)
        {
            var screen = await _screenRepo.Get(seatgenerateDTO.ScreenId);
            List<SeatOutputDTO> result = new List<SeatOutputDTO>();
            if (screen == null)
            {
                throw new EntityNotFoundException("Screen");
            }
            foreach (var rowRange in seatgenerateDTO.ColumnsPerRow.Keys)
            {
                int columnCount = seatgenerateDTO.ColumnsPerRow[rowRange];
                decimal price = seatgenerateDTO.RowPrices[rowRange];

                for (char row = rowRange[0]; row <= rowRange[2]; row++)
                {
                    for (int column = 1; column <= columnCount; column++)
                    {
                        string rowstring = row.ToString();
                        var seat = await _seatRepo.Add(new Seat
                        {
                            SeatNumber = $"{rowstring}{column}",
                            Row = rowstring,
                            Column = column,
                            Price = price,
                            ScreenId = seatgenerateDTO.ScreenId,
                        });
                        result.Add(MapSeatToOutputDTO(seat));
                    }
                }
            }
            screen.SeatingCapacity = result.Count;
            screen = await _screenRepo.Update(screen);
            return result;
        }
        /// <summary>
        /// Maps a seat entity to a seat output DTO.
        /// </summary>
        /// <param name="seat">The seat entity to be mapped.</param>
        /// <returns>A seat output DTO containing information mapped from the provided seat entity.</returns>

        private SeatOutputDTO MapSeatToOutputDTO(Seat seat)
        {
            SeatOutputDTO seatoutput = new SeatOutputDTO
            {
                SeatId = seat.SeatId,
                SeatNumber = seat.SeatNumber,
                Price = seat.Price,
            };
            return seatoutput;
        }
    }
}
