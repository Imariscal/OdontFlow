using Microsoft.EntityFrameworkCore;

namespace OdontFlow.Persistence.Contexts.Base;

public interface IReadOnlyUnitOfWork : IDisposable
{
    DbContext Context { get; }
}
