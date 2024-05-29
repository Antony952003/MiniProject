using MiniProjectAPI.Models;
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
        /// <summary>
        /// Retrieves the points balance for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user whose points balance is to be retrieved.</param>
        /// <returns>The points balance of the specified user.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the user's points record is not found.</exception>

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
        /// <summary>
        /// Redeems points for a user, providing a discount on their transaction.
        /// </summary>
        /// <param name="userId">The ID of the user whose points are to be redeemed.</param>
        /// <param name="pointsToRedeem">The number of points to redeem.</param>
        /// <param name="discountAmount">The discount amount to be applied.</param>
        /// <exception cref="InvalidOperationException">Thrown when the user does not have sufficient points to redeem.</exception>

        public async Task RedeemPoints(int userId, int? pointsToRedeem, decimal? discountAmount)
        {
            var userPoints = await _userPointsRepository.Get();
            var userPoint = userPoints.ToList().Find(x => x.UserId == userId);
            if (userPoint == null || userPoint.Points < pointsToRedeem)
            {
                throw new InvalidOperationException("Insufficient points to redeem.");
            }

            userPoint.Points -= (int)pointsToRedeem;
            userPoint.LastUpdated = DateTime.UtcNow;
            await _userPointsRepository.Update(userPoint);
        }
        /// <summary>
        /// Updates the points balance for a user, adding points that exceeded the limit.
        /// </summary>
        /// <param name="userId">The ID of the user whose points are to be updated.</param>
        /// <param name="pointsExceeded">The number of points exceeded beyond the limit.</param>

        public async Task UpdateExceededPoints(int userId, int pointsExceeded)
        {
            var userPoints = await _userPointsRepository.Get();
            var userPoint = userPoints.ToList().Find(x => x.UserId == userId);
            userPoint.Points += pointsExceeded;
            userPoint.LastUpdated = DateTime.UtcNow;
            await _userPointsRepository.Update(userPoint);
        }
    }
}
