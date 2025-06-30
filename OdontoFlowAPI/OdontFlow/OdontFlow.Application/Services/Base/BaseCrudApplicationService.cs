 
using OdontFlow.Domain.DTOs.Contracts;

namespace OdontFlow.Catalogos.Retail.Application.Services.Base;

public class BaseCrudApplicationService<TKey, TDTO>() :
    IBaseCrudApplicationService<TKey, TDTO> 
    where TDTO : class, IBaseCrudDTO<TKey> 
{
    public virtual Task AddAsync(TDTO dto) => throw new NotImplementedException();
    public virtual Task AddRangeAsync(IEnumerable<TDTO> dtos) => throw new NotImplementedException();
    public virtual Task<IEnumerable<TDTO>> GetAllAsync(params string[]? includes) =>  throw new NotImplementedException();
    public virtual Task<TDTO> GetAsync(TKey dtoId) => throw new NotImplementedException();
    public virtual Task ModifyAsync(TKey dtoId, TDTO dto) => throw new NotImplementedException();
    public virtual Task RemoveAsync(TKey id, bool applyPhysical = false) => throw new NotImplementedException();
    public virtual Task<bool> RemoveAsync(TDTO dto, bool applyPhysical = false) => throw new NotImplementedException();
    public virtual Task<bool> RemoveRangeAsync(IEnumerable<TDTO> dtos, bool applyPhysical = false) => throw new NotImplementedException();
}
 