using MovieBookingAPI.Models.DTOs.Payment;

namespace MovieBookingAPI.Interfaces
{
    public interface IPaymentService
    {
        public Task<PaymentReturnDTO> MakePayment(PaymentInputDTO paymentInputDTO);
    }
}
