using Microsoft.EntityFrameworkCore;
using OdontFlow.Persistence.Contexts.Base;

namespace OdontFlow.Infrastructure.Repositories.Base;

public class WriteOnlyUnitOfWork(IWriteOnlyContext context) : IWriteOnlyUnitOfWork, IDisposable
{
    private bool _disposed;

    public DbContext Context { get; private set; } = (DbContext)context;

    public virtual Task BeginTransactionAsync() =>
        Context.Database.BeginTransactionAsync();

    public Task CommitTransactionAsync() =>
        Context.Database.CommitTransactionAsync();

    public Task RollBackTransactionAsync() =>
        Context.Database.RollbackTransactionAsync();

    public virtual Task<int> SaveAsync() =>
        Context.SaveChangesAsync();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) Context?.Dispose();

        _disposed = true;
    }
}
