namespace MovieBookingAPI.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        readonly string message;
        public EntityNotFoundException(string value) {
            message = $"{value} not Found!!";
        }
        public override string Message => message;
    }
}
