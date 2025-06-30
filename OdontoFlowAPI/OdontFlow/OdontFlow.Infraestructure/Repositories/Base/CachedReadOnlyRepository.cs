using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OdontFlow.Domain.Entities.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Catalog.Infrastructure.Repositories.Base;

public class CachedReadOnlyRepository<TKey, TEntity>(
    IReadOnlyRepository<TKey, TEntity> repository,
    ILogger<IReadOnlyRepository<TKey, TEntity>> logger,
    IDistributedCache cache) :
    IReadOnlyRepository<TKey, TEntity> where TEntity : class, IBaseEntity<TKey>, IAuditable<TKey>
{
    // Set the TTL for cached data, check with Gama to see what could be a correct time
    private readonly DistributedCacheEntryOptions _options = 
        new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool onlyActive = true)
    {
        IEnumerable<TEntity> entities;

        var cachedEntities = await cache.GetStringAsync("ALL");
        if (!string.IsNullOrWhiteSpace(cachedEntities)) { entities = JsonSerializer.Deserialize<IEnumerable<TEntity>>(cachedEntities)!; }
        else
        {
            entities = await repository.GetAllAsync();
            if (entities.Any())
                await cache.SetStringAsync("ALL", JsonSerializer.Serialize(entities, _jsonOptions), _options);
        }

        return entities;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(params string[]? includes)
    {
        IEnumerable<TEntity> entities;
        string key = includes == null || includes.Length==0 ? "ALL" : $"ALL_{string.Join("_", includes)}";
        var cachedEntities = await cache.GetStringAsync(key);

        if (!string.IsNullOrWhiteSpace(cachedEntities)) { entities = JsonSerializer.Deserialize<IEnumerable<TEntity>>(cachedEntities)!; }
        else {
            entities = await repository.GetAllAsync(includes);

            if (entities.Any())
                await cache.SetStringAsync(key, JsonSerializer.Serialize(entities, _jsonOptions), _options);
        }
        return entities;
    }

    public Task<IEnumerable<TEntity>> GetAllMatchingAsync(Expression<Func<TEntity, bool>> filter, params string[] includes)
    {
        return repository.GetAllMatchingAsync(filter, includes);
    }

    public IQueryable<TEntity> GetAllMatchingQueryable(Expression<Func<TEntity, bool>> filter, params string[] includes)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity?> GetAsync(TKey entityId)
    {
        var cachedEntity = await cache.GetStringAsync(entityId!.ToString()!);
        if (!string.IsNullOrWhiteSpace(cachedEntity))
        {
            logger.LogInformation("Getting the data from redis cache");
            return JsonSerializer.Deserialize<TEntity>(cachedEntity);
        }

        logger.LogInformation("The info not exists on redis cache");
        var entity = await repository.GetAsync(entityId);
        await cache.SetStringAsync(entityId!.ToString()!, JsonSerializer.Serialize(entity), _options);
        return entity;
    }

    public async Task<TEntity?> GetAsync(TKey entityId, params string[]? includes)
    {
        string includesJoin = string.Empty;
        if (includes != null && includes.Length!=0) 
            includesJoin = string.Join(",", includes);

        var cachedEntity = await cache.GetStringAsync($"{entityId}{includesJoin}");
        if (!string.IsNullOrWhiteSpace(cachedEntity))
            return JsonSerializer.Deserialize<TEntity>(cachedEntity);

        var entity = await repository.GetAsync(entityId, includes);
        await cache.SetStringAsync($"{entityId}{includesJoin}", JsonSerializer.Serialize(entity), _options);
        return entity;
    }
}
