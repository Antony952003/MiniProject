namespace MovieBookingAPI.Exceptions
{
    public class NoEntitiesFoundException : Exception
    {
        readonly string message;
        public NoEntitiesFoundException(string value) {
            message = $"No {value}s are found!!";
        }
        public override string Message => message;
    }
}
