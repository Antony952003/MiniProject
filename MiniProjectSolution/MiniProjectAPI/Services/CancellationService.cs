using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Migrations;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Cancellation;

namespace MovieBookingAPI.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly IRepository<int, Cancellation> _cancellationRepo;
        private readonly IRepository<int, Booking> _bookingRepo;
        private readonly ITransaction _transactionService;


        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationService"/> class.
        /// </summary>
        /// <param name="bookingRepo">The repository for Booking entities.</param>
        /// <param name="cancellationRepo">The repository for Cancellation entities.</param>
        /// <param name="transactionService">The transaction service.</param>
        public CancellationService(IRepository<int, Booking> bookingRepo,
            IRepository<int, Cancellation> cancellationRepo,
            ITransaction transactionService) { 
            _cancellationRepo = cancellationRepo;
            _bookingRepo = bookingRepo;
            _transactionService = transactionService;
        }
        /// <summary>
        /// Processes the cancellation of a booking.
        /// </summary>
        /// <param name="cancellationInputDTO">The cancellation input DTO.</param>
        /// <returns>A cancellation return DTO containing information about the cancellation.</returns>
        public async Task<CancellationReturnDTO> ProcessCancellation(CancellationInputDTO cancellationInputDTO)
        {
            try
            {
                await _transactionService.BeginTransactionAsync();
                var booking = await _bookingRepo.Get(cancellationInputDTO.BookingId);

                if (booking == null)
                {
                    throw new EntityNotFoundException("Booking");
                }

                var showtime = booking.Showtime;
                var showtimeStartTime = showtime.StartTime;
                var currentTime = DateTime.Now;

                if (showtimeStartTime <= currentTime.AddMinutes(20))
                {
                    throw new CancellationNotAllowedException();
                }

                decimal refundAmount = 0m;
                var cancelledSeatNumbers = new List<string>();

                foreach (var seatNumber in cancellationInputDTO.SeatNumbers)
                {
                    var showtimeSeat = booking.ShowtimeSeats.FirstOrDefault(ss => ss.Seat.SeatNumber == seatNumber);
                    if (showtimeSeat == null)
                    {
                        throw new BookingDoesNotHaveSeatException(seatNumber);
                    }

                    var seatPrice = showtimeSeat.Seat.Price; 
                    if (showtimeStartTime >= currentTime.AddHours(2))
                    {
                        refundAmount += seatPrice * 0.75m; // 75% refund
                    }
                    else
                    {
                        refundAmount += seatPrice * 0.25m; // 25% refund
                    }
                    cancelledSeatNumbers.Add(seatNumber); // Add the seat number to the list
                }

                var cancellation = new Cancellation
                {
                    BookingId = cancellationInputDTO.BookingId,
                    CancellationDate = currentTime,
                    RefundAmount = refundAmount,
                    
                };

                cancellation = await _cancellationRepo.Add(cancellation);
                booking.CancelledAt = cancellation.CancellationDate;
                foreach (var seatnumber in cancellationInputDTO.SeatNumbers)
                {
                    var showtimeSeat = booking.ShowtimeSeats.FirstOrDefault(ss => ss.Seat.SeatNumber == seatnumber);
                    if (showtimeSeat == null)
                    {
                        throw new BookingDoesNotHaveSeatException(seatnumber);
                    }
                    showtimeSeat.Status = "Available";     // Reset seat status
                    showtimeSeat.CancellationId = cancellation.CancellationId;

                }
                booking = await _bookingRepo.Update(booking);

                await _transactionService.SaveChangesAsync();
                await _transactionService.CommitTransactionAsync();

                return new CancellationReturnDTO
                {
                    CancellationId = cancellation.CancellationId,
                    BookingId = booking.BookingId,
                    CancellationDate = cancellation.CancellationDate,
                    RefundAmount = cancellation.RefundAmount,
                    CancelledSeatNumbers = cancelledSeatNumbers
                };

            }
            catch(CancellationNotAllowedException ex)
            {
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
            catch(BookingDoesNotHaveSeatException ex)
            {
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
            catch(EntityNotFoundException ex)
            {
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
            catch(Exception ex)
            {
                await _transactionService.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
