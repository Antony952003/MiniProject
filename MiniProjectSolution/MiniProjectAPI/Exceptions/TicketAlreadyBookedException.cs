using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class TicketAlreadyBookedException : Exception
    {
        readonly string message;
        public TicketAlreadyBookedException()
        {
            message = "Ticket is Already Booked";
        }
        public override string Message => message;


    }
}