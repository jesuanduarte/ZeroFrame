using Microsoft.EntityFrameworkCore;
using ZeroFrame.Domain.Interfaces;
using ZeroFrame.Infra.Data.Context;

namespace ZeroFrame.Infra.Data.UnitOfWork
{
    //evita de acontecer um conflito no banco de dados que possa gerar um erro .
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Se tudo der certo, confirma a transação.
        // Se ocorrer qualquer erro, desfaz tudo com Rollback.
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    // Desfaz todas as alterações feitas dentro da transação
                   // e Lança o erro novamente para ser tratado pelas camadas superiores.
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await ExecuteInTransactionAsync(async () =>
            {
                await operation();
                return true;
            });
        }
    }
}
