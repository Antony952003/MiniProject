using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;

namespace MovieBookingAPI.Services
{
    public class PointsService : IPointsService
    {
        private readonly IUserPointRepository _userPointsRepository;

        public PointsService(IUserPointRepository userPointsRepository)
        {
            _userPointsRepository = userPointsRepository;
        }

        public async Task<int> GetPointsBalance(int userId)
        {
            var userPoints = await _userPointsRepository.Get();
            var userPoint = userPoints.ToList().Find(x => x.UserId == userId);
            if (userPoint == null)
            {
                throw new EntityNotFoundException("UserPoints");
            }
            return userPoint.Points;
        }

        public async Task RedeemPoints(int userId, int pointsToRedeem, decimal discountAmount)
        {
            var userPoints = await _userPointsRepository.Get();
            var userPoint = userPoints.ToList().Find(x => x.UserId == userId);
            if (userPoint == null || userPoint.Points < pointsToRedeem)
            {
                throw new InvalidOperationException("Insufficient points to redeem.");
            }

            userPoint.Points -= pointsToRedeem;
            userPoint.LastUpdated = DateTime.UtcNow;
            await _userPointsRepository.Update(userPoint);
        }
    }
}
