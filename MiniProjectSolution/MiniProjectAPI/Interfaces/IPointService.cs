namespace MovieBookingAPI.Interfaces
{
    public interface IPointsService
    {
        public Task<int> GetPointsBalance(int userId);
        public Task RedeemPoints(int userId, int pointsToRedeem, decimal discountAmount);
    }
}
