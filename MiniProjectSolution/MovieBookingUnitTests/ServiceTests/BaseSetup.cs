using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;
using NUnit.Framework;

namespace MovieBookingUnitTests.ServiceTests
{
    public class BasicSetup
    {
        public MovieBookingContext context;
        public IRepository<int, Movie> _movieRepo;
        public IRepository<int, Booking> _bookingRepo;
        public IRepository<int, BookingSnack> _bookingSnackRepo;
        public IRepository<int, Snack> _snackRepo;
        public IRepository<int, Cancellation> _cancellationRepo;
        public IRepository<int, Payment> _paymentRepo;
        public IRepository<int, Review> _reviewRepo;
        public IRepository<int, Screen> _screenRepo;
        public IRepository<int, Seat> _seatRepo;
        public IRepository<int, Showtime> _showtimeRepo;
        public IRepository<int, ShowtimeSeat> _showtimeSeatRepo;
        public IRepository<int, Theater> _theaterRepo;
        public IRepository<int, User> _userRepo;
        public IRepository<int, UserDetail> _userdetailsRepo;
        public IBookingService _bookingService;
        public IBookingSnackService _bookingSnackService;
        public ICancellationService _cancellationService;
        public IMovieService _movieService;
        public IPaymentService _paymentService;
        public IPointsService _pointsService;
        public IReviewService _reviewService;
        public IScreenService _screenService;
        public IShowtimeService _showtimeService;
        public IShowtimeSeatService _showtimeSeatService;
        public ITheaterService _theaterService;
        public ITokenService _tokenService;
        public ITransaction _transactionService;
        public IUserService _userService;
        public IUserPointRepository _userPointRepository;
        public ISnackService _snackService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
            context = new MovieBookingContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            _movieRepo = new MovieRepository(context);
            _bookingRepo = new BookingRepository(context);
            _bookingSnackRepo = new BookingSnackRepository(context);
            _snackRepo = new SnackRepository(context);
            _cancellationRepo = new CancellationRepository(context);
            _paymentRepo = new PaymentRepository(context);
            _reviewRepo = new ReviewRepository(context);
            _screenRepo = new ScreenRepository(context);
            _seatRepo = new SeatRepository(context);
            _showtimeRepo = new ShowtimeRepository(context);
            _showtimeSeatRepo = new ShowtimeSeatRepository(context);
            _theaterRepo = new TheaterRepository(context);
            _userPointRepository = new UserPointRepository(context);
            _userRepo = new UserRepository(context);
            _userdetailsRepo = new UserDetailRepository(context);

            _theaterService = new TheaterService(_theaterRepo);
            _showtimeService = new ShowtimeService(_showtimeRepo, _movieRepo, _screenRepo);
            _screenService = new ScreenService(_screenRepo, _theaterRepo);
            _movieService = new MovieService(_movieRepo);
            _snackService = new SnackService(_snackRepo);
            _showtimeSeatService = new ShowtimeSeatService(_showtimeSeatRepo, _screenRepo, _seatRepo);
            _paymentService = new PaymentService(_bookingRepo, _paymentRepo, _transactionService);
            _pointsService = new PointsService(_userPointRepository);
            _transactionService = new TransactionRepository(context);
            _bookingSnackService = new BookingSnackService(_bookingSnackRepo);
            _bookingService = new BookingService(
                _showtimeSeatRepo, _userRepo, _screenRepo, _showtimeRepo, _bookingRepo,
                _movieRepo, _seatRepo, _snackRepo, _theaterRepo, _userPointRepository,
                _pointsService, _showtimeSeatService, _bookingSnackService, _snackService, _transactionService);
            _cancellationService = new CancellationService(_bookingRepo, _cancellationRepo, _transactionService);

        }

        [TearDown]
        public void TearDown()
        {
            context.Database.CloseConnection();
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
