using OdontFlow.Application.BussinesProcess.Base.Contracts;
using Microsoft.Extensions.DependencyInjection; 

namespace OdontFlow.Catalogos.Retail.Application.BusinessProcess.Base;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ICommandHandler<TCommand> GetCommandHandler<TCommand>() where TCommand : class =>
        _serviceProvider.GetService<ICommandHandler<TCommand>>()!;

    public ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>() where TCommand : class, ICommand<TResult> =>
        _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>()!;

    public IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>() =>
        _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>()!;
}
