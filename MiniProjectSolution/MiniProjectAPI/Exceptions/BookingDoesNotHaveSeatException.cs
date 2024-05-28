using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class BookingDoesNotHaveSeatException : Exception
    {
        readonly string message;
        public BookingDoesNotHaveSeatException(string seatnumber)
        {
            message = $"{seatnumber} is not in the booked Seats";
        }
        public override string Message => message;

    }
}