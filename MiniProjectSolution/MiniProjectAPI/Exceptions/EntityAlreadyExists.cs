using System.Runtime.Serialization;

namespace MovieBookingAPI.Exceptions
{
    public class EntityAlreadyExists : Exception
    {
        readonly string message;
        public EntityAlreadyExists(string entity)
        {
            message = $"{entity} already exists!!";
        }
        public override string Message => message;


    }
}