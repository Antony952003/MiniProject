using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Payment;

namespace MovieBookingAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Booking> _bookingRepo;
        private readonly IRepository<int, Payment> _paymentRepo;
        private readonly ITransaction _transactionRepo;

        public PaymentService(IRepository<int, Booking> bookingRepo,
            IRepository<int, Payment> paymentRepo,
            ITransaction transactionRepo)
        {
            _bookingRepo = bookingRepo;
            _paymentRepo = paymentRepo;
            _transactionRepo = transactionRepo;
        }
        public async Task<PaymentReturnDTO> MakePayment(PaymentInputDTO paymentInputDTO)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var booking = await _bookingRepo.Get(paymentInputDTO.BookingId);
                Payment payment = null;
                if (booking == null)
                {
                    throw new EntityNotFoundException("Booking");
                }
                if (booking.PaymentStatus == "Success")
                {
                    throw new Exception("Payment already completed for this booking.");
                }
                // Calculate ticket price
                decimal ticketPrice = booking.ShowtimeSeats.Sum(bs => bs.Seat.Price);

                // Calculate snacks price
                decimal snacksPrice = booking.BookingSnacks.Sum(bs => bs.Snack.Price * bs.Quantity);

                // Calculate convenience fees
                decimal platformFee = 80m;
                decimal igstTax = 14.40m;
                decimal convenienceFee = platformFee + igstTax;

                // Calculate total order amount
                decimal orderTotal = ticketPrice + snacksPrice + convenienceFee;

                   // Create payment record
                payment = new Payment
                {
                    BookingId = booking.BookingId,
                    PaymentMethod = paymentInputDTO.PaymentMethod,
                    Amount = orderTotal,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Success",
                };

                payment = await _paymentRepo.Add(payment);
                // Update booking payment status
                booking.PaymentStatus = payment.PaymentStatus;
                await _bookingRepo.Update(booking);
                await _transactionRepo.SaveChangesAsync();
                await _transactionRepo.CommitTransactionAsync();

                return new PaymentReturnDTO
                {
                    PaymentId = payment.PaymentId,
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    PaymentStatus = booking.PaymentStatus,
                    TicketPrice = new Dictionary<string, decimal> { { "BaseAmount", ticketPrice } },
                    ConvenienceFees = new Dictionary<string, decimal> { { "PlatformFee", platformFee }, { "Integrated GST (IGST) @ 18%", igstTax } },
                    FoodBeverages = booking.BookingSnacks.ToDictionary(bs => bs.Snack.Name+" "+bs.Snack.Price+" Per Item", bs => bs.Quantity),
                    PaymentMethod = payment.PaymentMethod,
                    OrderTotal = payment.Amount
                };
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }

        }
    }
}
