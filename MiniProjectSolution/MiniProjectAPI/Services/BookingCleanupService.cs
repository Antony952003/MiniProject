using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Repositories;

namespace MovieBookingAPI.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public BookingCleanupService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_checkInterval, stoppingToken);
                await CleanupExpiredBookings();
            }
        }

        private async Task CleanupExpiredBookings()
        {

            using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var bookingRepo = scope.ServiceProvider.GetRequiredService<IRepository<int, Booking>>();
                    var showtimeSeatRepo = scope.ServiceProvider.GetRequiredService<IRepository<int, ShowtimeSeat>>();
                    var _userRepository = scope.ServiceProvider.GetRequiredService<IRepository<int, User>>();
                    var _userPointsRepository = scope.ServiceProvider.GetRequiredService<IUserPointRepository>();
                    var transactionService = scope.ServiceProvider.GetRequiredService<ITransaction>();
                    var expiredBookings = await GetExpiredPendingBookings(bookingRepo, TimeSpan.FromMinutes(1));

                    foreach (var booking in expiredBookings)
                    {
                        await transactionService.BeginTransactionAsync();
                        try
                        {
                            foreach (var showtimeSeat in booking.ShowtimeSeats)
                            {
                                showtimeSeat.Status = "Available";
                                showtimeSeat.BookingId = null;
                                await showtimeSeatRepo.Update(showtimeSeat);
                            }
                            int pointsEarned = 10;
                            if (booking.TotalPrice >= 2000) pointsEarned += 70;
                            pointsEarned += (int)(booking.TotalPrice / 1000) * 70;

                            await _userPointsRepository.DeductPoints(booking.UserId, pointsEarned);

                            await bookingRepo.Delete(booking.BookingId);

                            await transactionService.SaveChangesAsync();
                            await transactionService.CommitTransactionAsync();
                        }
                        catch
                        {
                            await transactionService.RollbackTransactionAsync();
                            throw;
                        }
                    }
                }
            
        }

        

        private async Task<IEnumerable<Booking>> GetExpiredPendingBookings(IRepository<int, Booking> bookingRepo, TimeSpan expirationTime)
        {
            var allBookings = await bookingRepo.Get();
            return allBookings.Where(b => b.PaymentStatus == "Pending" && DateTime.Now - b.CreatedAt >= expirationTime).ToList();
        }
    }

}
