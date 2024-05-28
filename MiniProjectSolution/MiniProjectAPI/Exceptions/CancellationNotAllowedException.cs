using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class CancellationNotAllowedException : Exception
    {
        readonly string message;
        public CancellationNotAllowedException()
        {
            message = "Cancellations are not allowed less than 20 minutes before the showtime";
        }
        public override string Message => message;

    }
}