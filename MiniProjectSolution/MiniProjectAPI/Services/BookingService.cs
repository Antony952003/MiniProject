using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IShowtimeSeatService _showtimeSeatService;
        private readonly IRepository<int, ShowtimeSeat> _showtimeSeatRepo;
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, Theater> _theaterRepo;
        private readonly IRepository<int, Movie> _movieRepo;
        private readonly IRepository<int, Seat> _seatRepo;
        private readonly IRepository<int, Snack> _snackRepo;
        private readonly IRepository<int, Screen> _screenRepo;
        private readonly IRepository<int, Showtime> _showtimeRepo;
        private readonly ISnackService _snackService;
        private readonly IRepository<int, Booking> _bookingRepo;
        private readonly IBookingSnackService _bookingSnackService;

        public BookingService(IRepository<int, ShowtimeSeat> showtimeSeatRepo,
            IRepository<int, User> userRepo,
            IRepository<int, Screen> screenRepo,
            IRepository<int, Showtime> showtimeRepo,
            IRepository<int, Booking> bookingRepo,
            IRepository<int, Movie> movieRepo,
            IRepository<int, Seat> seatRepo,
            IRepository<int, Snack> snackRepo,
        IRepository<int, Theater> theaterRepo,
            IShowtimeSeatService showtimeSeatService,
            IBookingSnackService bookingSnackService,
            ISnackService snackService
            ) {
            _showtimeSeatService = showtimeSeatService;
            _showtimeSeatRepo = showtimeSeatRepo;
            _userRepo = userRepo;
            _theaterRepo = theaterRepo;
            _movieRepo = movieRepo;
            _screenRepo = screenRepo;
            _showtimeRepo = showtimeRepo;
            _snackService = snackService;
            _bookingRepo = bookingRepo;
            _bookingSnackService = bookingSnackService;
            _seatRepo = seatRepo;
            _snackRepo = snackRepo;
        }

        public async Task<BookingReturnDTO> MakeBooking(BookingInputDTO bookingInputDTO)
        {
            var user = await _userRepo.Get(bookingInputDTO.UserId);
            if(user == null)
            {
                throw new EntityNotFoundException("User");
            }
            var showtime = await _showtimeRepo.Get(bookingInputDTO.ShowtimeId);
            if(showtime == null)
            {
                throw new EntityNotFoundException("Showtime");
            }
            //await _theaterRepo.Get();
            var showtimeSeatids = await _showtimeSeatService.GetShowtimeIdsForSeatNumbers(bookingInputDTO.BookingSeats, bookingInputDTO.ShowtimeId);
            if (!showtimeSeatids.Any())
            {
                throw new Exception("There is no seatId for the seatnumber provided check again");
            }
            string status = "Booked";
            var showtimetickets = await _showtimeSeatService.UpdateShowtimeSeatsStatus(bookingInputDTO.ShowtimeId,
                showtimeSeatids, status);
            decimal totalPrice = 0;
            totalPrice += CalculateTotalPriceForTickets(showtimetickets);
            if(bookingInputDTO.BookingSnacks.Any())
            {
                foreach (var item in bookingInputDTO.BookingSnacks)
                {
                    Snack snack = await _snackService.GetSnackIdByName(item.Key);
                    totalPrice += snack.Price * item.Value;
                }
            }
            Booking booking = new Booking()
            {
                UserId = bookingInputDTO.UserId,
                ShowtimeId = bookingInputDTO.ShowtimeId,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow,
                PaymentStatus = "Pending",
                BookingStatus = "Success",
            };
            booking = await _bookingRepo.Add(booking);
            showtimetickets = UpdateShowtimeSeatBooking(booking.BookingId, showtimetickets);
            var bookedSnacks = await MapBookingSnackToBooking(booking, bookingInputDTO.BookingSnacks);
            var screen = await _screenRepo.Get(showtime.ScreenId);
            var theater = await _theaterRepo.Get(screen.TheaterId);
            var movie = await _movieRepo.Get(showtime.MovieId);
            BookingReturnDTO bookingreturnDTO = await MapBookingToBookingReturnDTO(booking.BookingId, theater.Name, screen.Name, movie.Title
                , totalPrice, bookingInputDTO.BookingSeats, bookingInputDTO.BookingSnacks);
            return bookingreturnDTO;
        }

        private async Task<BookingReturnDTO> MapBookingToBookingReturnDTO(int bookingId, string theatername, string screenname, string moviename, decimal totalPrice, List<string> bookingSeats, Dictionary<string, int> bookingSnacks)
        {
            var booking = await _bookingRepo.Get(bookingId);
            BookingReturnDTO bookingreturnDTO = new BookingReturnDTO()
            {
                BookingId = bookingId,
                TotalPrice = totalPrice,
                TheaterName = theatername,
                MovieName = moviename,
                ScreenName = screenname,
                SnacksOrderedWithTickets = bookingSnacks,
                BookedTickets = bookingSeats,
                PaymentStatus = booking.PaymentStatus,
            };
            return bookingreturnDTO;
        }

        private async Task<List<BookingSnack>> MapBookingSnackToBooking(Booking booking, Dictionary<string, int> bookingSnacks)
        {
            List<BookingSnack> bookedSnacks = new List<BookingSnack>();
            foreach(var item in bookingSnacks)
            {
                Snack snack = await _snackService.GetSnackIdByName(item.Key);
                BookingSnack bookingsnack = await _bookingSnackService.SnackBooking(snack, item.Value, booking.BookingId);
                bookedSnacks.Add(bookingsnack);
            }
            return bookedSnacks;
        }

        private List<ShowtimeSeat> UpdateShowtimeSeatBooking(int bookingId, List<ShowtimeSeat> showtimetickets)
        {
            foreach(var item in showtimetickets)
            {
                item.BookingId = bookingId;
                _showtimeSeatRepo.Update(item);
            }
            return showtimetickets;
        }

        private decimal CalculateTotalPriceForTickets(List<ShowtimeSeat> showtimetickets)
        {
            decimal totalPrice = 0;
            foreach(var showtimeticket in showtimetickets)
            {
                totalPrice += showtimeticket.Seat.Price;
            }
            return totalPrice;
        }

        public async Task<List<BookingReturnDTO>> GetAllBookings()
        {
            var bookings = await _bookingRepo.Get();
            var AllBookings = new List<BookingReturnDTO>();
            bookings = bookings.ToList();
            foreach(var booking in bookings)
            {
                BookingReturnDTO bookingreturnDTO = await MapBookingToReturnDTO(booking);
                AllBookings.Add(bookingreturnDTO);
            }
            return AllBookings;

            
        }

        private async Task<BookingReturnDTO> MapBookingToReturnDTO(Booking booking)
        {
            var screen = await _screenRepo.Get(booking.Showtime.ScreenId);
            var theater = await _theaterRepo.Get(screen.TheaterId);
            var movie = await _movieRepo.Get(booking.Showtime.MovieId);
            var bookingsnacks = booking.BookingSnacks?.ToList();
            Dictionary<string, int> snacksdictionary = new Dictionary<string, int>();
            var tickets = new List<string>();
            foreach (var showtimeseat in booking.ShowtimeSeats)
            {
                var seat = await _seatRepo.Get(showtimeseat.SeatId);
                tickets.Add(seat.SeatNumber);
            }
            foreach (var bookingsnack in bookingsnacks)
            {
                var snack = await _snackRepo.Get(bookingsnack.SnackId);
                snacksdictionary.Add(snack.Name, bookingsnack.Quantity);
            }
            BookingReturnDTO bookingreturnDTO = await MapBookingToBookingReturnDTO(booking.BookingId, theater.Name, screen.Name, movie.Title
                , booking.TotalPrice, tickets, snacksdictionary);
            return bookingreturnDTO;
        }

        public async Task<BookingReturnDTO> GetBookingById(int bookingid)
        {
            var booking = await _bookingRepo.Get(bookingid);
            BookingReturnDTO bookingreturnDTO = await MapBookingToReturnDTO(booking);
            return bookingreturnDTO;
        }
    }
}
