namespace ZeroFrame.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
