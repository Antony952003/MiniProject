using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class AdminOnlyOperationException : Exception
    {
        private readonly string message;

        public AdminOnlyOperationException()
        {
            message = "UnAuthorize Action, Only Admin can do this Operation";
        }
        public override string Message => message;


    }
}