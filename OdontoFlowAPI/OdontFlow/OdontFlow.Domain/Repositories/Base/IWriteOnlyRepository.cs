
using OdontFlow.Domain.Entities.Base.Contracts;
 
namespace OdontFlow.Domain.Repositories.Base;
public interface IWriteOnlyRepository<TKey, TEntity> where TEntity : class, IBaseEntity<TKey>
{
    //  AddAsync
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    //  Modify
    Task Modify(TEntity entity);

    //  Remove
    Task<bool> Remove(TEntity entity, bool applyPhysical = false);
    Task<bool> RemoveRange(IEnumerable<TEntity> entities, bool applyPhysical = false);

    Task<bool> Remove(TKey entityId, bool applyPhysical = false);

    //  Save
    Task<int> SaveAsync(); 
}
