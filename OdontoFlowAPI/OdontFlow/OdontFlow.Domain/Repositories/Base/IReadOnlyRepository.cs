
using OdontFlow.Domain.Entities.Base.Contracts;
using System.Linq.Expressions;

namespace OdontFlow.Domain.Repositories.Base;

public interface IReadOnlyRepository<TKey, TEntity> where TEntity : class, IBaseEntity<TKey>
{
    Task<TEntity?> GetAsync(TKey entityId);
    Task<TEntity?> GetAsync(TKey entityId, params string[]? includes);
    Task<IEnumerable<TEntity>> GetAllAsync(bool onlyActive = true);
    Task<IEnumerable<TEntity>> GetAllAsync(params string[]? includes);
    Task<IEnumerable<TEntity>> GetAllMatchingAsync(
        Expression<Func<TEntity, bool>> filter,
        params string[] includes);
    IQueryable<TEntity> GetAllMatchingQueryable(
        Expression<Func<TEntity, bool>> filter,
        params string[] includes);
}