using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class InvalidTokenConfigurationException : Exception
    {
        readonly string message;
        public InvalidTokenConfigurationException()
        {
            message = "Invalid Token Configuration or Key is invalid";
        }
        public override string Message => message;
    }
}