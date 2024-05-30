using Microsoft.AspNetCore.Components.Forms;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;
using MovieBookingAPI.Repositories;
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
        private readonly ITransaction _transactionService;
        private readonly IRepository<int, Screen> _screenRepo;
        private readonly IRepository<int, Showtime> _showtimeRepo;
        private readonly ISnackService _snackService;
        private readonly IRepository<int, Booking> _bookingRepo;
        private readonly IBookingSnackService _bookingSnackService;
        private readonly IUserPointRepository _userPointsRepository;
        private readonly IPointsService _pointsService;

        public BookingService(IRepository<int, ShowtimeSeat> showtimeSeatRepo,
            IRepository<int, User> userRepo,
            IRepository<int, Screen> screenRepo,
            IRepository<int, Showtime> showtimeRepo,
            IRepository<int, Booking> bookingRepo,
            IRepository<int, Movie> movieRepo,
            IRepository<int, Seat> seatRepo,
            IRepository<int, Snack> snackRepo,
            IRepository<int, Theater> theaterRepo,
            IUserPointRepository userPointsRepository,
            IPointsService pointsService,
            IShowtimeSeatService showtimeSeatService,
            IBookingSnackService bookingSnackService,
            ISnackService snackService,
            ITransaction transaction
            ) {
            _showtimeSeatService = showtimeSeatService;
            _showtimeSeatRepo = showtimeSeatRepo;
            _userPointsRepository = userPointsRepository;
            _pointsService = pointsService;
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
            _transactionService = transaction;
        }

        /// <summary>
        /// Initiates the booking process, validates user and showtime, calculates total price,
        /// processes points redemption, creates a new booking, updates seat status,
        /// maps snacks to the booking, and awards points to the user.
        /// </summary>
        /// <param name="bookingInputDTO">Data transfer object containing booking details.</param>
        /// <returns>A BookingReturnDTO object with the booking details.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if user or showtime is not found.</exception>
        /// <exception cref="Exception">Thrown if no seat IDs are found for the provided seat numbers.</exception>
        public async Task<BookingReturnDTO> MakeBooking(BookingInputDTO bookingInputDTO)
        {
            try
            {
                await _transactionService.BeginTransactionAsync();

                var user = await _userRepo.Get(bookingInputDTO.UserId);
                if (user == null)
                {
                    throw new EntityNotFoundException("User");
                }
                var showtime = await _showtimeRepo.Get(bookingInputDTO.ShowtimeId);
                if (showtime == null)
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
                decimal totalPriceForTickets = 0;
                decimal totalPriceForSnacks = 0;
                totalPriceForTickets += CalculateTotalPriceForTickets(showtimetickets);
                if (bookingInputDTO.BookingSnacks.Any())
                {
                    foreach (var item in bookingInputDTO.BookingSnacks)
                    {
                        Snack snack = await _snackService.GetSnackIdByName(item.Key);
                        totalPriceForSnacks += snack.Price * item.Value;
                    }
                }
                totalPrice = totalPriceForTickets + totalPriceForSnacks;
                if (bookingInputDTO.PointsToRedeem != null)
                {
                    var discountAmount = bookingInputDTO.PointsToRedeem; // 1 point = 1 Rs
                    await _pointsService.RedeemPoints(bookingInputDTO.UserId,
                        bookingInputDTO.PointsToRedeem, discountAmount);

                    totalPrice -= (decimal)discountAmount;
                    if (totalPrice < 0)
                    {
                        var exceededpoints = totalPrice * -1;
                        totalPrice = 0;
                        await _pointsService.UpdateExceededPoints(bookingInputDTO.UserId, (int)exceededpoints);
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
                showtimetickets = await UpdateShowtimeSeatBooking(booking.BookingId, showtimetickets);
                var bookedSnacks = await MapBookingSnackToBooking(booking, bookingInputDTO.BookingSnacks);
                var screen = await _screenRepo.Get(showtime.ScreenId);
                var theater = await _theaterRepo.Get(screen.TheaterId);
                var movie = await _movieRepo.Get(showtime.MovieId);
                await AwardPoints(booking.UserId, totalPrice);
                await _transactionService.SaveChangesAsync();
                await _transactionService.CommitTransactionAsync();
                BookingReturnDTO bookingreturnDTO = await MapBookingToBookingReturnDTO(booking.BookingId, theater.Name, screen.Name, movie.Title
                    , totalPrice, bookingInputDTO.BookingSeats, bookingInputDTO.BookingSnacks);
                return bookingreturnDTO;
            }
            catch
            {
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
        }


        /// <summary>
        /// Maps booking details to a BookingReturnDTO object.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <param name="theatername">The name of the theater.</param>
        /// <param name="screenname">The name of the screen.</param>
        /// <param name="moviename">The title of the movie.</param>
        /// <param name="totalPrice">The total price of the booking.</param>
        /// <param name="bookingSeats">List of booked seat numbers.</param>
        /// <param name="bookingSnacks">Dictionary of booked snacks and their quantities.</param>
        /// <returns>A BookingReturnDTO object with the booking details.</returns>


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
        /// <summary>
        /// Maps snacks to a booking and creates a list of BookingSnack objects.
        /// </summary>
        /// <param name="booking">The booking entity.</param>
        /// <param name="bookingSnacks">Dictionary of snack names and their quantities.</param>
        /// <returns>A list of BookingSnack objects.</returns>
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
        /// <summary>
        /// Updates the booking ID for a list of showtime seats.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <param name="showtimetickets">List of showtime seats to update.</param>
        /// <returns>The updated list of showtime seats.</returns>
        private async Task<List<ShowtimeSeat>> UpdateShowtimeSeatBooking(int bookingId, List<ShowtimeSeat> showtimetickets)
        {
            foreach(var item in showtimetickets)
            {
                item.BookingId = bookingId;
                await _showtimeSeatRepo.Update(item);
            }
            return showtimetickets;
        }
        /// <summary>
        /// Calculates the total price for a list of showtime seats.
        /// </summary>
        /// <param name="showtimetickets">List of showtime seats.</param>
        /// <returns>The total price of the tickets.</returns>
        private decimal CalculateTotalPriceForTickets(List<ShowtimeSeat> showtimetickets)
        {
            decimal totalPrice = 0;
            foreach(var showtimeticket in showtimetickets)
            {
                totalPrice += showtimeticket.Seat.Price;
            }
            return totalPrice;
        }
        /// <summary>
        /// Retrieves all bookings and maps them to a list of BookingReturnDTO objects.
        /// </summary>
        /// <returns>A list of BookingReturnDTO objects with booking details.</returns>
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
        /// <summary>
        /// Maps a booking entity to a BookingReturnDTO object.
        /// </summary>
        /// <param name="booking">The booking entity.</param>
        /// <returns>A BookingReturnDTO object with the booking details.</returns>
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
        /// <summary>
        /// Retrieves a booking by its ID and maps it to a BookingReturnDTO object.
        /// </summary>
        /// <param name="bookingid">The ID of the booking.</param>
        /// <returns>A BookingReturnDTO object with the booking details.</returns>
        public async Task<BookingReturnDTO> GetBookingById(int bookingid)
        {
            var booking = await _bookingRepo.Get(bookingid);
            if (booking == null)
                throw new EntityNotFoundException("Booking");
            BookingReturnDTO bookingreturnDTO = await MapBookingToReturnDTO(booking);
            return bookingreturnDTO;
        }
        /// <summary>
        /// Retrieves all expired pending bookings based on the provided expiration time.
        /// </summary>
        /// <param name="expirationTime">The time span for expiration.</param>
        /// <returns>An enumerable of expired pending bookings.</returns>
        public async Task<IEnumerable<Booking>> GetExpiredPendingBookings(TimeSpan expirationTime)
        {
            var allBookings = await _bookingRepo.Get();
            return allBookings.Where(b => b.PaymentStatus == "Pending" && DateTime.Now - b.CreatedAt >= expirationTime).ToList();
        }
        // <summary>
        /// Awards points to a user based on the total amount spent.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="totalAmount">The total amount spent by the user.</param>
        private async Task AwardPoints(int userId, decimal totalAmount)
        {
            const int basePoints = 10; // Base points for any booking
            const int bonusPointsPer1000 = 70; // Bonus points for each 1000 Rs above the first 2000 Rs
            const int thresholdAmount = 2000; // Threshold amount for extra points

            var pointsToAward = basePoints;

            if (totalAmount > thresholdAmount)
            {
                pointsToAward += bonusPointsPer1000; // Add 70 points for crossing 2000 Rs
                var additionalThousands = (int)((totalAmount - thresholdAmount) / 1000);
                pointsToAward += additionalThousands * bonusPointsPer1000; // Add 70 points for each additional 1000 Rs
            }

            var userPoints = await _userPointsRepository.Get();
            UserPoint userPoint = null;
            if (userPoints != null)
            {
                userPoint = userPoints.ToList().Find(x => x.UserId == userId);
            }
            if (userPoint == null)
            {
                userPoint = new UserPoint
                {
                    UserId = userId,
                    Points = pointsToAward,
                    LastUpdated = DateTime.UtcNow
                };
                await _userPointsRepository.Add(userPoint);
            }
            else
            {
                userPoint.Points += pointsToAward;
                userPoint.LastUpdated = DateTime.UtcNow;
                await _userPointsRepository.Update(userPoint);
            }
        }
        /// <summary>
        /// Retrieves all bookings for a specific user and maps them to a list of BookingReturnDTO objects.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of BookingReturnDTO objects with the user's booking details.</returns>
        /// <exception cref="NoEntitiesFoundException">Thrown if no bookings are found for the user.</exception>
        public async Task<IEnumerable<BookingReturnDTO>> GetAllUserBookings(int userId)
        {
            var bookings = await _bookingRepo.Get();
            bookings = bookings.ToList().FindAll(x => x.UserId == userId);
            if(bookings == null)
            {
                throw new NoEntitiesFoundException("Booking");
            }
            List<BookingReturnDTO> bookingreturns = new List<BookingReturnDTO>();
            foreach(var booking in bookings)
            {
                var result = await MapBookingToReturnDTO(booking);
                bookingreturns.Add(result);
            }
            return bookingreturns;
            
        }
    }
}
