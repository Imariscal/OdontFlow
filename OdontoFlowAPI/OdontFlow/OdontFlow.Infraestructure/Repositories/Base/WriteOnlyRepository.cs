using Microsoft.EntityFrameworkCore;
using OdontFlow.Domain.Entities.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;

namespace OdontFlow.Infrastructure.Repositories.Base;

public class WriteOnlyRepository<TKey, TEntity> :
    IWriteOnlyRepository<TKey, TEntity> where TEntity : class, IBaseEntity<TKey>, IAuditable<TKey>
{
    protected readonly IWriteOnlyUnitOfWork _uow;
    protected readonly DbSet<TEntity> _dbSet;

    public WriteOnlyRepository(IWriteOnlyUnitOfWork uow)
    {
        _uow = uow;
        _dbSet = _uow.Context.Set<TEntity>();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        entity.Active = true;
        entity.Deleted = false;
        entity.CreationDate = DateTime.Now;
        entity.CreatedBy = "Super visor";

        entity.LastModificationDate = DateTime.Now;
        entity.LastModifiedBy = "Super visor";

        await _dbSet.AddAsync(entity);
        await _uow.SaveAsync();
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.Active = true;
            entity.Deleted = false;
            entity.CreationDate = DateTime.Now;
            entity.CreatedBy = "Super visor";

            entity.LastModificationDate = DateTime.Now;
            entity.LastModifiedBy = "Super visor";

            await _dbSet.AddAsync(entity);
        }

        await _uow.SaveAsync();
    }

    public virtual async Task Modify(TEntity entity)
    {
        entity.LastModificationDate = DateTime.Now;
        entity.LastModifiedBy = "Super visor";

        // 1. Verificamos si la entidad ya está trackeada
        var local = _dbSet.Local.FirstOrDefault(e => e.Id.Equals(entity.Id));

        if (local != null)
        {
            // Si ya está en el contexto, actualizamos sus valores
            _uow.Context.Entry(local).CurrentValues.SetValues(entity);
        }
        else
        {
            // Si no está en el contexto, la adjuntamos y marcamos como modificada
            _dbSet.Attach(entity);
            _uow.Context.Entry(entity).State = EntityState.Modified;
        }

        // 2. Forzamos tracking de hijos (por ejemplo, Items)
        foreach (var navigation in _uow.Context.Entry(entity).Navigations)
        {
            if (navigation.Metadata.IsCollection && navigation.CurrentValue is IEnumerable<object> children)
            {
                foreach (var child in children)
                {
                    var childEntry = _uow.Context.Entry(child);

                    if (childEntry.State == EntityState.Detached)
                    {
                        childEntry.State = EntityState.Added;
                    }
                }
            }
        }

        // 3. Guardamos
        await _uow.SaveAsync();
    }




    public virtual async Task<bool> Remove(TEntity entity, bool applyPhysical = false)
    {
        if (applyPhysical) _dbSet.Remove(entity);
        else
        {
            entity.DeletionDate  = DateTime.Now;
            entity.DeletedBy = "Super visor";
            entity.Deleted = true;

            _dbSet.Update(entity);
        }

        await _uow.SaveAsync();
        return true;
    }

    public virtual async Task<bool> Remove(TKey entityId, bool applyPhysical = false)
    {
        var entity = _dbSet.Find(entityId)??throw new ArgumentNullException(nameof(entityId));
        await Remove(entity, applyPhysical);

        return true;
    }

    public virtual async Task<bool> RemoveRange(IEnumerable<TEntity> entities, bool applyPhysical = false)
    {
        if (applyPhysical) _dbSet.RemoveRange(entities);
        else
        {
            foreach (var entity in entities)
            {
                entity.Active = false;
                entity.DeletionDate = DateTime.Now;
                entity.DeletedBy = "Super visor";
                entity.Deleted = true;
                _dbSet.Remove(entity);
            }
        }

        await _uow.SaveAsync();
        return true;
    }

    public async Task<int> SaveAsync() => await _uow.SaveAsync();
}