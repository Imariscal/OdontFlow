

using OdontFlow.Domain.DTOs.Contracts;

public interface IBaseCrudApplicationService<TKey, TDTO>
    where TDTO : class, IBaseCrudDTO<TKey>
{
    //  Get Async
    Task<TDTO> GetAsync(TKey dtoId);
    Task<IEnumerable<TDTO>> GetAllAsync(params string[]? includes);

    //  AddAsync
    Task AddAsync(TDTO dto);
    Task AddRangeAsync(IEnumerable<TDTO> dtos);

    //  Modify
    Task ModifyAsync(TKey dtoId, TDTO dto);

    //  Remove
    Task RemoveAsync(TKey id, bool applyPhysical = false);
    Task<bool> RemoveAsync(TDTO dto, bool applyPhysical = false);
    Task<bool> RemoveRangeAsync(IEnumerable<TDTO> dtos, bool applyPhysical = false);
}
