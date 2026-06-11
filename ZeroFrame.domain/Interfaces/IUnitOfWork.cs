namespace ZeroFrame.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        // Define um contrato para executar operações dentro de uma transação.
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
