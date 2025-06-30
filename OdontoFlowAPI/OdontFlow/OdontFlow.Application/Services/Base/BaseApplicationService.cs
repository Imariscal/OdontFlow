using Microsoft.Extensions.Logging;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.Services.Base.Contracts;
using OdontFlow.Domain.DTOs.Contracts;
using OdontFlow.Domain.Entities.Base.Contracts;

namespace OdontFlow.Catalogos.Retail.Application.Services.Base;

public abstract class BaseApplicationService<BaseDTO, TEntity, TKey>(
    IMediator mediator,
    ILogger<IBaseApplicationService<BaseDTO>> logger) : IBaseApplicationService<BaseDTO> 
        where BaseDTO : class, IBaseDTO
        where TEntity : class, IBaseEntity<TKey>
{
    protected readonly IMediator _mediator = mediator;
    protected readonly ILogger<IBaseApplicationService<BaseDTO>> _logger = logger;
}