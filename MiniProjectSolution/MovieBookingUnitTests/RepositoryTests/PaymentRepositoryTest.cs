using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Models;
using MovieBookingAPI.Repositories;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingUnitTests.RepositoryTests
{
    public class PaymentRepositoryTest
    {
        private DbContextOptions<MovieBookingContext> _options;
        private MovieBookingContext _context;
        private PaymentRepository _paymentRepository;
        private BookingRepository _bookingRepository;
        private MovieRepository _movieRepository;
        private ScreenRepository _screenRepository;
        private ShowtimeRepository _showtimeRepository;
        private TheaterRepository _theaterRepository;
        private SeatRepository _seatRepository;
        private ShowtimeSeatRepository _showtimeseatRepository;

        private Booking _testBooking;

        [SetUp]
        public async Task Setup()
        {
            _options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new MovieBookingContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _paymentRepository = new PaymentRepository(_context);
            _bookingRepository = new BookingRepository(_context);
            _movieRepository = new MovieRepository(_context);
            _screenRepository = new ScreenRepository(_context);
            _showtimeRepository = new ShowtimeRepository(_context);
            _seatRepository = new SeatRepository(_context);
            _showtimeseatRepository = new ShowtimeSeatRepository(_context);
            _theaterRepository = new TheaterRepository(_context);

            var movie = new Movie()
            {
                Title = "Fight Club",
                Description = "Unhappy with his capitalistic lifestyle, a white-collared insomniac forms an underground fight club with Tyler, a careless soap salesman",
                Cast = "Edward Norton, Brad Pitt, Jared Leto",
                Director = "David Fincher",
                DurationInHours = 2.5,
                Genre = "Thriller",
            };

            movie = await _movieRepository.Add(movie);

            Theater theater = new Theater()
            {
                Name = "Rohini Cinemas",
                Location = "Vadapalani, Chennai",
            };
            theater = await _theaterRepository.Add(theater);
            Screen screen = new Screen()
            {
                Name = "PXL",
                SeatingCapacity = 230,
                TheaterId = theater.TheaterId,
            };
            screen = await _screenRepository.Add(screen);
            Showtime showtime = new Showtime()
            {
                MovieId = movie.MovieId,
                ScreenId = screen.ScreenId,
                StartTime = DateTime.Parse("2015-05-16T05:50:06"),
            };
            showtime = await _showtimeRepository.Add(showtime);
            Seat seat = new Seat()
            {
                Row = "A",
                Column = 1,
                Price = 170,
                SeatNumber = "A1",
                ScreenId = screen.ScreenId,
            };
            seat = await _seatRepository.Add(seat);
            ShowtimeSeat showtimeSeat = new ShowtimeSeat()
            {
                ShowtimeId = showtime.ShowtimeId,
                SeatId = seat.SeatId,
                Status = "Booked",
            };
            showtimeSeat = await _showtimeseatRepository.Add(showtimeSeat);
            Booking booking = new Booking()
            {
                CreatedAt = DateTime.Now,
                PaymentStatus = "Pending",
                ShowtimeId = showtime.ShowtimeId,
                TotalPrice = 170,
                BookingStatus = "Success"
            };
            _testBooking = await _bookingRepository.Add(booking);
            showtimeSeat.BookingId = _testBooking.BookingId;
            showtimeSeat = await _showtimeseatRepository.Update(showtimeSeat);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        
    }
}
