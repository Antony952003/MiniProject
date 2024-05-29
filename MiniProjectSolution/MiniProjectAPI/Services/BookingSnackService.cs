using MiniProjectAPI.Models;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Services
{
    public class BookingSnackService : IBookingSnackService
    {
        private readonly IRepository<int, BookingSnack> _bookingSnackRepo;
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingSnackService"/> class.
        /// </summary>
        /// <param name="bookingSnackRepo">The repository for BookingSnack entities.</param>
        public BookingSnackService(IRepository<int, BookingSnack> bookingSnackRepo) { 
            _bookingSnackRepo = bookingSnackRepo;
        }
        /// <summary>
        /// Creates a new booking snack entry and saves it to the repository.
        /// </summary>
        /// <param name="snack">The snack entity being booked.</param>
        /// <param name="quantity">The quantity of the snack being booked.</param>
        /// <param name="bookingId">The ID of the associated booking.</param>
        /// <returns>The created BookingSnack entity.</returns>
        public async Task<BookingSnack> SnackBooking(Snack snack, int quantity, int bookingId)
        {
            BookingSnack bookingSnack = new BookingSnack()
            {
                BookingId = bookingId,
                Quantity = quantity,
                SnackId = snack.SnackId,
            };
            bookingSnack = await _bookingSnackRepo.Add(bookingSnack);
            return bookingSnack;

        }
    }
}
