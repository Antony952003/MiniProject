using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class TicketAlreadyBookedException : Exception
    {
        readonly string message;
        public TicketAlreadyBookedException(string seatnumber)
        {
            message = $"Ticket {seatnumber} is Already Booked";
        }
        public override string Message => message;


    }
}