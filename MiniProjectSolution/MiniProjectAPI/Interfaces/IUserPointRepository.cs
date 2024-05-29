using MovieBookingAPI.Models;

namespace MovieBookingAPI.Interfaces
{
    public interface IUserPointRepository : IRepository<int, UserPoint>
    {
        public Task<UserPoint> GetUserPointsByUserId(int userId);

        public Task DeductPoints(int userId, int points);
    }

}
