using OdontFlow.Persistence.Contexts.Base;

namespace OdontFlow.Infrastructure.Repositories.Base;

public interface IWriteOnlyUnitOfWork : IReadOnlyUnitOfWork
{
    Task<int> SaveAsync();
    Task BeginTransactionAsync();
    Task RollBackTransactionAsync();
    Task CommitTransactionAsync();
}