namespace MovieBookingAPI.Interfaces
{

    public interface ITransaction
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task SaveChangesAsync();

    }
}
