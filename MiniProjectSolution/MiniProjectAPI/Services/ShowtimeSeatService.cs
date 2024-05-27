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
                    result.Add(MapShowtimeSeatWithReturnDTO(showtimeSeat, seat));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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

        public async Task<List<ShowtimeSeat>> UpdateShowtimeSeatsStatus(int showtimeId, List<int> showtimeseatIds, string status)
        {
            var AllShowtimeseats = await _showtimeseatRepo.Get();
            if(!AllShowtimeseats.Any()) throw new NoEntitiesFoundException("ShowtimeSeat");
            var showtimeSeats = AllShowtimeseats.ToList()
                .Where(ss => ss.ShowtimeId == showtimeId && showtimeseatIds.Contains(ss.ShowtimeSeatId))
                .ToList();
            foreach( var showtimeSeat in showtimeSeats)
            {
                if (showtimeSeat.Status == status) throw new TicketAlreadyBookedException();
                showtimeSeat.Status = status;
                await _showtimeseatRepo.Update(showtimeSeat);
            }
            return showtimeSeats;
            
        }

        private ShowtimeSeatReturnDTO MapShowtimeSeatWithReturnDTO(ShowtimeSeat showtimeSeat, Seat seat)
        {
            ShowtimeSeatReturnDTO showtimeseatreturn = new ShowtimeSeatReturnDTO()
            {
                ShowtimeSeatId = showtimeSeat.ShowtimeSeatId,
                SeatId = showtimeSeat.SeatId,
                Price = seat.Price,
                SeatNumber = seat.SeatNumber,
                Status = showtimeSeat.Status,
            };
            return showtimeseatreturn;
        }
    }
}
