using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniProjectAPI.Models;
using Moq;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Repositories;
using MovieBookingAPI.Services;

namespace MovieBookingUnitTests
{
    public class BasicSetup
    {

        public MovieBookingContext context;
        IRepository<int, Movie> _movieRepo;
        IRepository<int, Booking> _bookingRepo;
        IRepository<int, BookingSnack> _bookingSnackRepo;
        IRepository<int, Snack> _snackRepo;
        IRepository<int, Cancellation> _cancellationRepo;
        IRepository<int, Payment> _paymentRepo;
        IRepository<int, Review> _reviewRepo;
        IRepository<int, Screen> _screenRepo;
        IRepository<int, Seat> _seatRepo;
        IRepository<int, Showtime> _showtimeRepo;
        IRepository<int, ShowtimeSeat> _showtimeSeatRepo;
        IRepository<int, Theater> _theaterRepo;
        IRepository<int, User> _userRepo;
        IRepository<int, UserDetail> _userdetailsRepo;
        IBookingService _bookingService;
        IBookingSnackService _bookingSnackService;
        ICancellationService _cancellationService;
        IMovieService _movieService;
        IPaymentService _paymentService;
        IPointsService _pointsService;
        IReviewService _reviewService;
        IScreenService _screenService;
        IShowtimeService _showtimeService;
        IShowtimeSeatService _showtimeSeatService;
        ITheaterService _theaterService;
        ITokenService _tokenService;
        ITransaction _transactionService;
        IUserService _userService;
        IUserPointRepository _userPointRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory")
                .Options;
            context = new MovieBookingContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureDeleted();
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
            _userRepo = new UserRepository(context);
            _userdetailsRepo = new UserDetailRepository(context);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is my JWT signature for the Movie Booking App which has to be a bit long like 512 bytes I guess this works");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object);
            _userService = new UserService(_userdetailsRepo,_userRepo,_tokenService);

        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}