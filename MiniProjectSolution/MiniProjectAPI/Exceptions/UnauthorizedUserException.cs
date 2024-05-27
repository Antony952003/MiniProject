using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class UnauthorizedUserException : Exception
    {
        readonly string message;
        public UnauthorizedUserException(string msg)
        {
            message = msg;  
        }
        public override string Message => message;


    }
}