using MiniProjectAPI.Models;

namespace MovieBookingAPI.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}
