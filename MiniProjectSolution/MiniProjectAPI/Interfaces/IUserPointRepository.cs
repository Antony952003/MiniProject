using MovieBookingAPI.Models;

namespace MovieBookingAPI.Interfaces
{
    public interface IUserPointRepository : IRepository<int, UserPoint>
    {
        public Task<IEnumerable<UserPoint>> GetUserPointsByUserId(int userId);
    }

}
