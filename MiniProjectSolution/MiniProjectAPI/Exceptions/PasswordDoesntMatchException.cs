using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class PasswordDoesntMatchException : Exception
    {
        readonly string message;
        public PasswordDoesntMatchException()
        {
            message = "Both the passwords doesn't match";
        }
        public override string Message => message;

    }
}